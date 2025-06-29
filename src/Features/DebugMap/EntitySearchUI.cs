// code borrowed from https://github.com/EverestAPI/Everest/blob/dev/Celeste.Mod.mm/Mod/UI/OuiMapSearch.cs
// The MIT License (MIT), Copyright (c) 2018 Everest Team

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GameHelper.Features.DebugMap;

public class EntitySearchUI : Entity {
    private IDictionary<string, List<int[]>> SearchIndex => mode switch {
        EntitySearch.Mode.Entities => GameHelper.Session.EntityIndex,
        EntitySearch.Mode.Triggers => GameHelper.Session.TriggerIndex,
        EntitySearch.Mode.Groups => GameHelper.Session.GroupIndex,
        _ => null,
    };
    private List<string> RecentSearch => GameHelper.Session.RecentSearch;

    public List<OuiChapterSelectIcon> OuiIcons;
    private EntitySearchMenu menu;
    private const float onScreenX = 960f;
    private const float offScreenX = 2880f;
    private float alpha = 0f;
    private float cursorStart;
    private readonly List<TextMenu.Item> items = new();
    private bool searching;
    private string search = "";
    private string searchPrev = "";
    private TextMenu.Item searchTitle;
    private bool searchConsumedButton;
    private int itemCount;
    private EntitySearch.Mode mode = EntitySearch.Mode.Entities;
    private bool sortCount = false;
    private bool previousSort = false;
    private TextMenu.SubHeader resultHeader;
    private TextMenu.SubHeader otherHeader;
    private TextMenu.Button buttonMode;
    private TextMenu.Button buttonSort;

    public bool Searching {
        get => searching;
        set {
            if(value != searching) { // Prevent multiple subscriptions
                if(value) {
                    TextInput.OnInput += OnTextInput;
                    cursorStart = Scene.TimeActive;
                } else {
                    TextInput.OnInput -= OnTextInput;
                }
            }
            searching = value;
        }
    }

    public EntitySearchUI() {
        Add(new Coroutine(Enter()));
    }

    private void cleanExit() {
        search = "";
        Audio.Play(SFX.ui_main_button_back);
        Add(new Coroutine(Leave()));
    }

    public void OnTextInput(char c) {
        if(c == (char) 13) {
            // Enter
            Scene.OnEndOfFrame += () => {
                switchMenu();

                // execute directly if only one item
                if(itemCount == 1) {
                    items[0].OnPressed.Invoke();
                }
            };

            goto ValidButton;

        } else if(c == (char) 8) {
            // Backspace - trim.
            if(search.Length > 0) {
                search = search[..^1];
                Audio.Play(SFX.ui_main_rename_entry_backspace);
                goto ValidButton;
            } else {
                if(Input.MenuCancel.Pressed) {
                    Audio.Play(SFX.ui_main_button_back);
                    switchMenu();
                    goto ValidButton;
                }
                return;
            }

        } else if(c == ' ') {
            // Space - append.
            if(search.Length > 0) {
                if(ActiveFont.Measure(search + c + "_").X < 542)
                    search += c;
            }
            Audio.Play(SFX.ui_main_rename_entry_space);
            goto ValidButton;

        } else if(!char.IsControl(c)) {
            // Any other character - append.
            if(ActiveFont.FontSize.Characters.ContainsKey(c)) {
                Audio.Play(SFX.ui_main_rename_entry_char);
                if(ActiveFont.Measure(search + c + "_").X < 542)
                    search += c;
                goto ValidButton;
            } else {
                goto InvalidButton;
            }
        }

        return;

        ValidButton:
        searchConsumedButton = true;
        MInput.Disabled = true;
        MInput.UpdateNull();
        return;

        InvalidButton:
        Audio.Play(SFX.ui_main_button_invalid);
        return;
    }

    private void ReloadMenu() {
        Vector2 position = Vector2.Zero;

        int selected = -1;
        if(menu != null) {
            position = menu.Position;
            selected = menu.Selection;
        }

        menu = new EntitySearchMenu();
        menu.leftMenu.Add(searchTitle = new TextMenu.Header(Dialog.Clean("maplist_search")) {
            Selectable = true
        });
        menu.leftMenu.Add(new TextMenu.Button("") { Disabled = true });
        menu.leftMenu.Add(new TextMenu.Button("") { Disabled = true });
        menu.leftMenu.Add(new TextMenu.SubHeader("Click to switch"));
        menu.leftMenu.Add(buttonMode = new TextMenu.Button("Entities") {
            OnPressed = () => {
                mode = (EntitySearch.Mode) (((int) mode + 1) % 3);
                buttonMode.Label = mode.ToString();
                if(mode == EntitySearch.Mode.Groups) {
                    previousSort = sortCount;
                    sortCount = true;
                    buttonSort.Label = "Amount";
                    buttonSort.Disabled = true;
                } else {
                    sortCount = previousSort;
                    buttonSort.Label = sortCount ? "Amount" : "Name";
                    buttonSort.Disabled = false;
                }
                ReloadItems();
            }
        });
        menu.leftMenu.Add(new TextMenu.SubHeader("Sort by"));
        menu.leftMenu.Add(buttonSort = new TextMenu.Button("Name") {
            OnPressed = () => {
                previousSort = sortCount = !sortCount;
                buttonSort.Label = sortCount ? "Amount" : "Name";
                ReloadItems();
            }
        });
        menu.rightMenu.Add(resultHeader = new TextMenu.SubHeader(""));
        ReloadItems();

        if(selected >= 0) {
            menu.Selection = selected;
            menu.Position = position;
        }
    }

    private void ReloadItems() {
        itemCount = 0;

        menu.rightMenu.BatchMode = true;

        items.ForEach(i => menu.rightMenu.Remove(i));
        items.Clear();
        if(otherHeader != null) {
            menu.rightMenu.Remove(otherHeader);
            otherHeader = null;
        }

        // find alias matches
        IEnumerable<string> aliases = EntitySearchData.Aliases
            .Where(pair => pair.Key.Contains(search, StringComparison.CurrentCultureIgnoreCase))
            .Select(pair => pair.Value);

        bool recent = false;
        foreach(string key in RecentSearch.Reverse<string>()) {
            if(SearchIndex.TryGetValue(key, out List<int[]> list) && key.Contains(search, StringComparison.CurrentCultureIgnoreCase)) {
                recent = true;
                itemCount++;
                TextMenu.Button button = new(key + " (" + list.Count + ")") {
                    OnPressed = () => {
                        search = "";
                        Inspect(key);
                    },
                };
                menu.rightMenu.Add(button);
                items.Add(button);
            }
        }

        if(recent) {
            menu.rightMenu.Add(otherHeader = new TextMenu.SubHeader(""));
        }

        IEnumerable<KeyValuePair<string, List<int[]>>> orderedIndex = sortCount ? SearchIndex.OrderBy(pair => -pair.Value.Count) : SearchIndex;
        foreach(KeyValuePair<string, List<int[]>> keyValue in orderedIndex) {
            if(!RecentSearch.Contains(keyValue.Key) && (keyValue.Key.Contains(search, StringComparison.CurrentCultureIgnoreCase) || aliases.Any(tag => tag.Equals(keyValue.Key)))) {
                itemCount++;
                TextMenu.Button button = new(keyValue.Key + " (" + keyValue.Value.Count + ")") {
                    OnPressed = () => {
                        search = "";
                        Inspect(keyValue.Key);
                    },
                };
                menu.rightMenu.Add(button);
                items.Add(button);
            }
        }

        menu.rightMenu.BatchMode = false;
        menu.rightMenu.Selection = itemCount > 0 ? 1 : 0;

        string info = string.Format(itemCount == 1 ? Dialog.Get("maplist_results_singular") : Dialog.Get("maplist_results_plural"), itemCount)
            + (itemCount > 0 && mode == EntitySearch.Mode.Groups ? ", Groups with [n] are a predefined list of Vanilla entities" : "");
        if(recent) {
            resultHeader.Title = "Recent searches";
            otherHeader.Title = info;
        } else {
            resultHeader.Title = info;
        }

        if(menu.rightMenu.Height > menu.rightMenu.ScrollableMinSize) {
            menu.rightMenu.Position.Y = menu.rightMenu.ScrollTargetY;
        }

        // Don't allow pressing any buttons while searching
        items.ForEach(i => i.Disabled = menu.leftFocused || !menu.Focused);
    }

    public IEnumerator Enter() {
        Searching = true;

        ReloadMenu();

        menu.rightMenu.MinWidth = menu.rightMenu.Width;

        menu.Visible = Visible = true;
        menu.Focused = false;

        for(float p = 0f; p < 1f; p += Engine.DeltaTime * 4f) {
            menu.X = offScreenX + -1920f * Ease.CubeOut(p);
            alpha = Ease.CubeOut(p);
            yield return null;
        }

        menu.Focused = true;
    }

    public IEnumerator Leave() {
        MInput.Disabled = false;

        menu.Focused = false;
        Searching = false;

        Audio.Play(SFX.ui_main_whoosh_large_out);

        for(float p = 0f; p < 1f; p += Engine.DeltaTime * 4f) {
            menu.X = onScreenX + 1920f * Ease.CubeIn(p);
            alpha = 1f - Ease.CubeIn(p);
            yield return null;
        }

        menu.Visible = Visible = false;
        Searching = false;
        RemoveSelf();
    }

    private bool switchMenu() {
        bool nextIsLeft = !menu.leftFocused;
        if(menu.Focused && (nextIsLeft || items.Count > 0)) {
            menu.leftFocused = nextIsLeft;
            MInput.Disabled = nextIsLeft;
            menu.currentMenu.Selection = nextIsLeft ? 0 : 1;
            if(nextIsLeft) {
                Audio.Play(SFX.ui_main_button_toggle_off);
            } else {
                Audio.Play(SFX.ui_main_button_toggle_on);
            }
            return true;
        }
        return false;
    }

    public override void Update() {
        Searching = menu != null && menu.leftFocused && menu.leftMenu.Selection == 0;

        if(Searching) {
            if(MInput.Keyboard.Pressed(Keys.Delete)) {
                if(search.Length > 0) {
                    search = "";
                    Audio.Play(SFX.ui_main_rename_entry_backspace);
                } else {
                    cleanExit();
                }
                searchConsumedButton = true;
                MInput.UpdateNull();
            }
            MInput.Disabled = searchConsumedButton;
        }
        searchConsumedButton = false;

        if(menu != null && menu.Focused) {
            if(search != searchPrev) {
                ReloadItems();
                searchPrev = search;
            }

            if(Input.MenuCancel.Pressed || Input.Pause.Pressed || Input.ESC.Pressed) {
                if(Searching && search != "") {
                    if(!switchMenu()) {
                        cleanExit();
                    }
                } else {
                    cleanExit();
                }
            }

            if(Input.MenuRight.Pressed) {
                if(!menu.leftFocused)
                    return;
                switchMenu();
            }

            if(Input.MenuLeft.Pressed) {
                if(menu.leftFocused)
                    return;
                switchMenu();
            }
        }

        base.Update();

        // Don't allow pressing any buttons while searching
        menu?.rightMenu.Items.ForEach(i => i.Disabled = menu.leftFocused || !menu.Focused);
        menu?.Update();
    }

    public override void Render() {
        if(menu == null) return;

        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.ScreenMatrix);
        if(alpha > 0f) {
            Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * alpha * 0.8f);
        }
        TextMenu leftMenu = menu.leftMenu;
        // search window
        Color searchBarColor = Color.DarkSlateGray;
        searchBarColor.A = 128;
        bool cursor = Searching && menu.Focused && (Scene.TimeActive - cursorStart) * 2 % 2 < 1;
        Vector2 pos = leftMenu.Position + new Vector2(-200f, leftMenu.GetYOffsetOf(searchTitle) - 180f);
        Draw.Rect(pos + new Vector2(-8f, 32f), 416, (int) (ActiveFont.HeightOf("l" + search) + 8) * -1, searchBarColor);
        ActiveFont.DrawOutline(search + (cursor ? "_" : ""), pos, new Vector2(0f, 0.5f), Vector2.One * 0.75f, Color.White * leftMenu.Alpha, 2f, Color.Black * (leftMenu.Alpha * leftMenu.Alpha * leftMenu.Alpha));
        Draw.SpriteBatch.End();

        menu.Render();
        base.Render();
    }

    public override void Removed(Scene scene) {
        base.Removed(scene);
        Searching = false;
        MInput.Disabled = false;
    }

    public override void SceneEnd(Scene scene) {
        Searching = false;
        MInput.Disabled = false;
    }

    private void Inspect(string key) {
        if(!RecentSearch.Contains(key)) {
            RecentSearch.Add(key);
            if(RecentSearch.Count > 5) {
                RecentSearch.RemoveAt(0);
            }
        }

        Audio.Play(SFX.ui_main_button_select);
        Scene.Add(new EntitySearchRenderer(key, mode));
        RemoveSelf();
    }
}