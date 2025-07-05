/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *                                                                                                                       *
 *                          This project replicates the camera system used in Stardew Valley.                            *
 *       It allows pixel art games to render nicely at any resolution by smoothing gently on non-integer scales.         *
 *                      The window resizing behavior also follow the same system as Stardew Valley.                      *
 *                                              Free to use for any purpose.                                             *
 *                                                                                                                       *
 *    Note: The textures used are from Haunted Chocolatier by ConcernedApe, used here for educational purposes only.     *
 *                                (source: https://www.hauntedchocolatier.net/media/)                                    *
 *                                                                                                                       *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace StardewCamera;

public class Game1 : Game
{
    public const int MIN_WINDOW_WIDTH = 1280;
    public const int MIN_WINDOW_HEIGHT = 720;
    public const float MIN_ZOOM = 0.75f;
    public const float MAX_ZOOM = 2f;
    public const float MIN_SPEED = 1f;
    public const float MAX_SPEED = 20f;

    private readonly GraphicsDeviceManager _graphics;

    private KeyboardState _previousKeyboardState;
    private CameraViewport _cameraViewport;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _cameraScreen;
    private Texture2D _background1;
    private Texture2D _background2;
    private Texture2D _background3;
    private Texture2D _background4;
    private Texture2D _background5;
    private Texture2D _background6;
    private Texture2D _background7;
    private Texture2D _background8;
    private Texture2D _background9;
    private bool _isResizing;
    private float _cameraSpeed = 7.5f;
    private float _zoom = 1f;


    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = Math.Clamp(value, MIN_ZOOM, MAX_ZOOM);
            UpdateScreenScale();
        }
    }

    public float CameraSpeed
    {
        get => _cameraSpeed;
        set => _cameraSpeed = Math.Clamp(value, MIN_SPEED, MAX_SPEED);
    }


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    protected override void Initialize()
    {
        _cameraViewport = new(0, 0, MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT);

        UpdateWindowSize(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT, false);
        UpdateScreenScale();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _background1 = Content.Load<Texture2D>("bg1");
        _background2 = Content.Load<Texture2D>("bg2");
        _background3 = Content.Load<Texture2D>("bg3");
        _background4 = Content.Load<Texture2D>("bg4");
        _background5 = Content.Load<Texture2D>("bg5");
        _background6 = Content.Load<Texture2D>("bg6");
        _background7 = Content.Load<Texture2D>("bg7");
        _background8 = Content.Load<Texture2D>("bg8");
        _background9 = Content.Load<Texture2D>("bg9");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

        // Fullscreen
        if (IsKeyPressed(currentKeyboardState, Keys.F11))
            UpdateWindowSize(MIN_WINDOW_WIDTH, MIN_WINDOW_HEIGHT, !_graphics.IsFullScreen);

        // Zoom
        if (IsKeyPressed(currentKeyboardState, Keys.Up)) Zoom += 0.1f;
        if (IsKeyPressed(currentKeyboardState, Keys.Down)) Zoom -= 0.1f;

        // Camera speed
        if (IsKeyPressed(currentKeyboardState, Keys.Right)) CameraSpeed += 1f;
        if (IsKeyPressed(currentKeyboardState, Keys.Left)) CameraSpeed -= 1f;

        // Movements
        Vector2 dir = Vector2.Zero;

        if (Keyboard.GetState().IsKeyDown(Keys.Z)) dir.Y = -1;
        if (Keyboard.GetState().IsKeyDown(Keys.Q)) dir.X = -1;
        if (Keyboard.GetState().IsKeyDown(Keys.S)) dir.Y = 1;
        if (Keyboard.GetState().IsKeyDown(Keys.D)) dir.X = 1;

        if (dir != Vector2.Zero) dir = Vector2.Normalize(dir);

        // Here we move the camera directly, but in a game, we would move the player and have the camera following him.
        _cameraViewport.Position += dir * _cameraSpeed;

        _previousKeyboardState = currentKeyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        /* * * * * * * * * * * * * * * * * *
         * Draw sprites on the game screen *
         * * * * * * * * * * * * * * * * * */

        GraphicsDevice.SetRenderTarget(_cameraScreen);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp); // When drawing on the camera screen we use PointClamp.

        /* Example of how to draw an sprite correctly */

        Vector2 bgWorldPosition = Vector2.Zero;

        // Make sure to floor the world position before drawing to prevent sprite jittering with lerped camera movements.
        // Note: if you're experiencing texture bleeding on animated sprite using a spritesheet: set a padding of one pixel between each frame.
        Vector2 bgRoundedPosition = new((float)Math.Floor(bgWorldPosition.X), (float)Math.Floor(bgWorldPosition.Y));

        // Convert the floored world position to camera space.
        Vector2 bgScreenPosition = _cameraViewport.ToCameraSpace(bgRoundedPosition);

        // Every sprite is rendered 4 times larger.
        float scale = 4f;

        Rectangle srcRect = new(0, 0, _background1.Width, _background1.Height);

        _spriteBatch.Draw(_background1, bgScreenPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        DrawOtherBackgrounds(); // To make it cleaner. All the backgrounds follow the same logic as the previous one.

        _spriteBatch.End();

        /* * * * * * * * * * * * * * * * * * * * * * *
         * Draw the camera screen on the back buffer *
         * * * * * * * * * * * * * * * * * * * * * * */

        GraphicsDevice.SetRenderTarget(null);

        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp); // When drawing on the back buffer we use LinearClamp
        _spriteBatch.Draw(_cameraScreen, Vector2.Zero, _cameraScreen.Bounds, Color.White, 0f, Vector2.Zero, Zoom, SpriteEffects.None, 1f);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void UpdateWindowSize(int windowWidth, int windowHeight, bool fullscreen, bool borderless = false)
    {
        if (fullscreen)
        {
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = windowWidth;
            _graphics.PreferredBackBufferHeight = windowHeight;
            _graphics.IsFullScreen = false;
            Window.IsBorderless = borderless;
        }
        _graphics.ApplyChanges();
    }

    private void OnClientSizeChanged(object sender, EventArgs e)
    {
        if (!_isResizing)
        {
            _isResizing = true;

            // Resize the window if it's too small
            if (GraphicsDevice.Viewport.Width < MIN_WINDOW_HEIGHT)
            {
                _graphics.PreferredBackBufferWidth = MIN_WINDOW_WIDTH;
                _graphics.PreferredBackBufferHeight = MIN_WINDOW_HEIGHT;
                _graphics.ApplyChanges();
            }

            UpdateScreenScale();

            _isResizing = false;
        }
    }

    private void UpdateScreenScale()
    {
        Vector2 previousCenter = _cameraViewport.Center;

        int sw = (int)Math.Ceiling(GraphicsDevice.Viewport.Width * (1f / Zoom));
        int sh = (int)Math.Ceiling(GraphicsDevice.Viewport.Height * (1f / Zoom));

        // Avoid odd resolutions
        if (sw % 2 != 0) sw++;
        if (sh % 2 != 0) sh++;

        _cameraScreen?.Dispose(); // Don't forget to dipose the render target.
        _cameraScreen = new(_graphics.GraphicsDevice, sw, sh);

        _cameraViewport.Width = _cameraScreen.Width;
        _cameraViewport.Height = _cameraScreen.Height;

        // Center the camera
        _cameraViewport.Position = previousCenter - new Vector2(_cameraViewport.Width / 2f, _cameraViewport.Height / 2f);
    }

    private bool IsKeyPressed(KeyboardState current, Keys key) => current.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);

    private void DrawOtherBackgrounds()
    {
        float scale = 4f;
        Rectangle srcRect = new(0, 0, 480, 270);

        Vector2 bgPosition = _cameraViewport.ToCameraSpace(new(-1920, 0)); // Note: Positions don't need to be floored here.
        _spriteBatch.Draw(_background2, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(1920, 0));
        _spriteBatch.Draw(_background3, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(0, -1080));
        _spriteBatch.Draw(_background4, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(0, 1080));
        _spriteBatch.Draw(_background5, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(-1920, 1080));
        _spriteBatch.Draw(_background6, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(1920, 1080));
        _spriteBatch.Draw(_background7, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(1920, -1080));
        _spriteBatch.Draw(_background8, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);

        bgPosition = _cameraViewport.ToCameraSpace(new(-1920, -1080));
        _spriteBatch.Draw(_background9, bgPosition, srcRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 1f);
    }
}
