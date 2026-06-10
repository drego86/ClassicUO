// SPDX-License-Identifier: BSD-2-Clause

using ClassicUO.Assets;
using ClassicUO.Game.Managers;
using ClassicUO.Game.Scenes;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using MathHelper = ClassicUO.Utility.MathHelper;

namespace ClassicUO.Game.UI.Gumps.CharCreation
{
    internal class CreateCharSelectionCityGump : Gump
    {
        private readonly List<CityControl> _cityControls = new List<CityControl>();
        private readonly string[] _cityNames = { "Felucca", "Trammel", "Ilshenar", "Malas", "Tokuno", "Ter Mur" };
        private readonly Label _facetName;
        private readonly HtmlControl _htmlControl;
        private readonly LoginScene _scene;
        private CityInfo _selectedCity;
        private readonly byte _selectedProfession;

        private readonly Point[] _townButtonsText =
        {
            new Point(105, 130),//new haven
            new Point(245, 90),//yew
            new Point(165, 200),//minoc
            new Point(395, 160),//britain
            new Point(50, 50),//moonglow 200, 305  vesper
            new Point(335, 250),//trinsic
            new Point(160, 395),//jhelom
            new Point(100, 250),//skara brea
            new Point(270, 130)//vesper
        };

        public CreateCharSelectionCityGump(World world, byte profession, LoginScene scene) : base(world, 0, 0)
        {
            CanMove = false;
            CanCloseWithRightClick = false;
            CanCloseWithEsc = false;

            _scene = scene;
            _selectedProfession = profession;

            CityInfo city;

            if (Client.Game.UO.Version >= ClientVersion.CV_70130)
            {
                city = scene.GetCity(0);
            }
            else
            {
                city = scene.GetCity(3);

                if (city == null)
                {
                    city = scene.GetCity(0);
                }
            }

            if (city == null)
            {
                Log.Error(ResGumps.NoCityFound);
                Dispose();

                return;
            }

            uint map = 0;

            if (city.IsNewCity)
            {
                map = city.Map;
            }

            _facetName = new Label
            (
                "",
                true,
                0x0481,
                font: 0,
                style: FontStyle.BlackBorder
            )
            {
                X = 240,
                Y = 440
            };

            if (Client.Game.UO.Version >= ClientVersion.CV_70130)
            {
                // We force 0x15D9 so it always shows the Felucca background
                Add(new GumpPic(62, 54, 0x15D9, 0));
                Add(new GumpPic(57, 49, 0x15DF, 0));

                // Force the label to use the first name in your list (Firebriar)
                _facetName.Text = _cityNames[1];
            }
            /* if (Client.Game.UO.Version >= ClientVersion.CV_70130)
             {
                 Add(new GumpPic(62, 54, (ushort) (0x15D9 + map), 0));
                 Add(new GumpPic(57, 49, 0x15DF, 0));
                 _facetName.Text = _cityNames[map];
             }
             else
             {
                 Add(new GumpPic(57, 49, 0x1598, 0));
                 _facetName.IsVisible = false;
             }*/

            Add(_facetName);

            Add
            (
                new Button((int) Buttons.PreviousScreen, 0x15A1, 0x15A3, 0x15A2)
                {
                    X = 586,
                    Y = 445,
                    ButtonAction = ButtonAction.Activate
                }
            );

            Add
            (
                new Button((int) Buttons.Finish, 0x15A4, 0x15A6, 0x15A5)
                {
                    X = 610,
                    Y = 445,
                    ButtonAction = ButtonAction.Activate
                }
            );


            _htmlControl = new HtmlControl
            (
                452,
                60,
                175,
                367,
                true,
                true,
                ishtml: true,
                text: city.Description
            );

            Add(_htmlControl);

            for (int i = 0; i < scene.Cities.Length; i++)
            {
                CityInfo c = scene.GetCity(i);
            
                // --- ADD THIS DEBUG BLOCK ---
                //Console.WriteLine($"[DEBUG] Index: {i} | City: {c.City} | IsNewCity: {c.IsNewCity}");
                // ----------------------------
                if (c == null)
                {
                    continue;
                }

                int x = 0;
                int y = 0;

                if (c.IsNewCity)
                {
                    uint cityFacet = c.Map;

                    if (cityFacet > 5)
                    {
                        cityFacet = 5;
                    }

                    x = 62 + MathHelper.PercetangeOf(Client.Game.UO.FileManager.Maps.MapsDefaultSize[cityFacet, 0] - 2048, c.X, 383);//was 62
                    y = 54 + MathHelper.PercetangeOf(Client.Game.UO.FileManager.Maps.MapsDefaultSize[cityFacet, 1], c.Y, 384);

                    switch (i)
                    {
                        case 0: x -= 50;y -= 50; break; // Tallenbrook
                        case 1: x -= 60; break; // Eaglevale
                        case 2: x -= 30; break; // Aelburn
                        case 3: x -= 50; break; // Firebriar
                        case 4: x -= 20; break; // Urkim
                        case 5: x -= 75; break; // Silverash
                        case 6: x -= 25;y -= 10; break; // Wildmist
                        case 7: x -= 25; break; // Alaspar
                        case 8: x -= 25; break; // Leviathos
                    }
                }
                else if (i < _townButtonsText.Length)
                {
                    x = _townButtonsText[i].X;

                    y = _townButtonsText[i].Y;
                }

                CityControl control = new CityControl(c, x, y, i);
                Add(control);
                _cityControls.Add(control);
            }

            SetCity(city);
        }


        private void SetCity(int index)
        {
            SetCity(_scene.GetCity(index));
        }

        private void SetCity(CityInfo city)
        {
            if (city == null)
            {
                return;
            }

            _selectedCity = city;
            _htmlControl.Text = city.Description;
            SetFacet(city.Map);
        }

        private void SetFacet(uint index)
        {
            if (Client.Game.UO.Version < ClientVersion.CV_70130)
            {
                return;
            }

            // Force it to always use index 0 ("Felucca")
            _facetName.Text = _cityNames[0];
        }

        /* private void SetFacet(uint index)
         {

            if (Client.Game.UO.Version < ClientVersion.CV_70130)
             {
                 return;
             }

             if (index >= _cityNames.Length)
             {
                 index = (uint) (_cityNames.Length - 1);
             }

             _facetName.Text = _cityNames[index];
         }*/


        public override void OnButtonClick(int buttonID)
        {
            CharCreationGump charCreationGump = UIManager.GetGump<CharCreationGump>();

            if (charCreationGump == null)
            {
                return;
            }

            switch ((Buttons) buttonID)
            {
                case Buttons.PreviousScreen:
                    charCreationGump.StepBack(_selectedProfession > 0 ? 2 : 1);

                    return;

                case Buttons.Finish:

                    if (_selectedCity != null)
                    {
                        charCreationGump.SetCity(_selectedCity.Index);
                    }

                    charCreationGump.CreateCharacter(_selectedProfession);
                    charCreationGump.IsVisible = false;

                    return;
            }

            if (buttonID >= 2)
            {
                buttonID -= 2;
                SetCity(buttonID);
                SetSelectedLabel(buttonID);
            }
        }

        private void SetSelectedLabel(int index)
        {
            for (int i = 0; i < _cityControls.Count; i++)
            {
                _cityControls[i].IsSelected = index == i;
            }
        }

        private enum Buttons
        {
            PreviousScreen,
            Finish
        }

        private class CityControl : Control
        {
            private readonly Button _button;
            private bool _isSelected;
            private readonly HoveredLabel _label;

            public CityControl(CityInfo c, int x, int y, int index)
            {
                CanMove = false;


                Add
                (
                    _button = new Button(2 + index, 0x04B9, 0x04BA, 0x04BA)
                    {
                        ButtonAction = ButtonAction.Activate,
                        X = x,
                        Y = y
                    }
                );

                y -= 20;

                _label = new HoveredLabel
                (
                    c.City,
                    false,
                    0x0058,
                    0x0099,
                    0x0481,
                    font: 3
                )
                {
                    X = x,
                    Y = y,
                    Tag = index
                };

                if (_label.X + _label.Width >= 383)
                {
                    _label.X -= 60;
                }

                _label.MouseUp += (sender, e) =>
                {
                    _label.IsSelected = true;
                    int idx = (int) _label.Tag;
                    OnButtonClick(idx + 2);
                };

                Add(_label);
            }


            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        _label.IsSelected = value;
                        _button.IsClicked = value;
                    }
                }
            }


            public override void Update()
            {
                base.Update();

                if (!_isSelected)
                {
                    _button.IsClicked = _button.MouseIsOver || _label.MouseIsOver;
                    _label.ForceHover = _button.MouseIsOver;
                }
            }

            public override bool Contains(int x, int y)
            {
                Control c = null;
                _label.HitTest(x, y, ref c);

                if (c != null)
                {
                    return true;
                }

                _button.HitTest(x, y, ref c);

                return c != null;
            }
        }
    }
}