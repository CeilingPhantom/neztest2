﻿using System;
using System.Collections.Generic;
using Nez;
using NezTest2.Scenes;
using NezTest2.Units;

namespace NezTest2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class NezTest2 : Core
    {
        public static Dictionary<string, Func<Scene>> Scenes { get; private set; }

        public NezTest2() : base(640, 640)
        {
            Window.AllowUserResizing = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            Scene.SetDefaultDesignResolution(320, 320, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            
            Scenes = new Dictionary<string, Func<Scene>>
            {
                { "1", () => new Scene1() },
            };
            
            Scene = new Scene1();
        }
    }
}
