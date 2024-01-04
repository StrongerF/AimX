using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static Settings.ControlSettings.MouseSensitivity;

namespace Settings
{
    public class GameSettings
    {
        public VideoSettings Video { get; set; }
        public ControlSettings Control { get; set; }
        

        public GameSettings() { }
        public GameSettings(VideoSettings videoSettings, ControlSettings controlSettings)
        {
            Video = videoSettings;
            Control = controlSettings;
        }
        public GameSettings(GameSettings gameSettings)
        {
            Video = new VideoSettings(gameSettings.Video);
            Control = new ControlSettings(gameSettings.Control);
        }
    }

    public class VideoSettings
    {
        public bool IsChanged { get; set; } = false;


        public static readonly Resolution[] Resolutions = Screen.resolutions;
        public static readonly List<FullScreenMode> FullScreenModes = new List<FullScreenMode>
        {
            FullScreenMode.ExclusiveFullScreen,
            FullScreenMode.FullScreenWindow,
            FullScreenMode.Windowed
        };

        public Resolution Resolution { get; set; }
        public FullScreenMode FullscreenMode { get; set; }


        public VideoSettings(Resolution resolution, FullScreenMode fullscreenMode)
        {
            Resolution = resolution;
            FullscreenMode = fullscreenMode;
        }
        public VideoSettings(VideoSettings videoSettings)
        {
            Resolution = videoSettings.Resolution;
            FullscreenMode = videoSettings.FullscreenMode;
        }



        public static FullScreenMode GetFullScreenMode(int index)
        {
            return index switch
            {
                0 => FullScreenMode.ExclusiveFullScreen,
                1 => FullScreenMode.FullScreenWindow,
                2 => FullScreenMode.Windowed,
                _ => FullScreenMode.FullScreenWindow,
            };
        }
    }

    public class ControlSettings
    {
        public bool IsChanged { get; set; }


        public MouseSensitivity Sensitivity { get; set; }


        public ControlSettings(MouseSensitivity sensitivity)
        {
            Sensitivity = sensitivity;
        }
        public ControlSettings(ControlSettings controlSettings)
        {
            Sensitivity = new MouseSensitivity(controlSettings.Sensitivity);
        }


        public class MouseSensitivity
        {
            public static readonly List<Game> Games = new List<Game>
            {
                new Game
                {
                    Title = "Default",
                    Type = Game.GameType.Default,
                    SensitivityMultiplier = 1,
                },
                new Game
                {
                    Title = "CS2",
                    Type = Game.GameType.CS2,
                    SensitivityMultiplier = 0.44f
                },
                new Game
                {
                    Title = "Valorant",
                    Type = Game.GameType.Valorant,
                    SensitivityMultiplier = 1.399999f
                }
            };

            public static List<string> GameTitles
            {
                get => Games.Select(g => g.Title).ToList();
            }


            public Game SourceGame { get; set; }
            public float SourceGameSensitivity { get; set; }
            public float ModifiedSensitivity
            {
                get => ConvertFromGame(SourceGame, SourceGameSensitivity);
            }



            public MouseSensitivity(Game game, float gameSensitivity)
            {
                SourceGame = game;
                SourceGameSensitivity = (float)Math.Round(gameSensitivity, 3);
            }
            public MouseSensitivity(MouseSensitivity mouseSensitivity)
            {
                SourceGame = mouseSensitivity.SourceGame;
                SourceGameSensitivity = mouseSensitivity.SourceGameSensitivity;
            }


            private static float ConvertFromGame(Game game, float gameSensitivity)
            {
                return gameSensitivity * game.SensitivityMultiplier;
            }

            public void ConvertToAnotherGame(Game game)
            {
                float newGameSensitivity = ModifiedSensitivity / game.SensitivityMultiplier;

                SourceGame = game;
                SourceGameSensitivity = newGameSensitivity;
            }

            public static float ConvertBetweenGames(Game sourceGame, Game targetGame, float gameSensitivity)
            {
                float newGameSensitivity = ConvertFromGame(sourceGame, gameSensitivity) / targetGame.SensitivityMultiplier;
                return newGameSensitivity;
            }


            public class Game
            {
                public enum GameType
                {
                    Default,
                    CS2,
                    Valorant
                }

                public GameType Type;
                public string Title;
                public float SensitivityMultiplier;
                
            }

        }
    }
}



