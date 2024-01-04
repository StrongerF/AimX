using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

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
        public static Resolution[] Resolutions = Screen.resolutions;
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
        public MouseSensitivity Sensitivity { get; set; }


        public ControlSettings(MouseSensitivity sensitivity)
        {
            Sensitivity = sensitivity;
        }

        /// <summary>
        /// Initializes ControlSettings with a MouseSensitivity based on the provided game string and sensitivity
        /// </summary>
        /// <param name="game">Selected game</param>
        /// <param name="sensitivity">Sensitivity in selected game</param>
        public ControlSettings(string game, float sensitivity)
        {
            Sensitivity = new MouseSensitivity(game, sensitivity);
        }
        public ControlSettings(ControlSettings controlSettings)
        {
            Sensitivity = new MouseSensitivity(controlSettings.Sensitivity);
        }


        
    }

    public class MouseSensitivity
    {
        public static readonly Dictionary<string, float> GameSensMultipliers = new Dictionary<string, float>
            {
                { "Default", 1 },
                { "CS2", 0.44f },
                { "Valorant", 1.399999f }
            };

        public static List<string> GameTitles
        {
            get => GameSensMultipliers.Keys.ToList();
        }

        public string SourceGame { get; set; }
        public float SourceGameSensitivity { get; set; }
        public float ModifiedSensitivity
        {
            get => ConvertFromGame(SourceGame, SourceGameSensitivity);
        }



        public MouseSensitivity(string game, float gameSensitivity)
        {
            if (GameTitles.Contains(game))
            {
                SourceGame = game;
            }
            else
            {
                SourceGame = GameTitles.First();
            }
            SourceGameSensitivity = (float)Math.Round(gameSensitivity, 3);
        }
        public MouseSensitivity(MouseSensitivity mouseSensitivity)
        {
            SourceGame = mouseSensitivity.SourceGame;
            SourceGameSensitivity = mouseSensitivity.SourceGameSensitivity;
        }


        private static float ConvertFromGame(string game, float gameSensitivity)
        {
            if (GameSensMultipliers.TryGetValue(game, out float mult))
            {
                return gameSensitivity * mult;
            }
            return gameSensitivity;
        }

        public void ConvertToAnotherGame(string game)
        {
            if (GameSensMultipliers.TryGetValue(game, out float mult))
            {
                float newGameSensitivity = ModifiedSensitivity / mult;

                SourceGame = game;
                SourceGameSensitivity = newGameSensitivity;
            }
        }

        public static float ConvertBetweenGames(string sourceGame, string targetGame, float gameSensitivity)
        {
            if (GameSensMultipliers.TryGetValue(targetGame, out float mult))
            {
                float newGameSensitivity = ConvertFromGame(sourceGame, gameSensitivity) / mult;
                return newGameSensitivity;
            }
            return gameSensitivity;
        }

    }
}



