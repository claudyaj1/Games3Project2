using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// 
/// <author>Andrew Claudy</author>
/// <email>andrew.claudy@live.com</email>
/// <!--
/// [current version] 1.6.1
/// 1.6.1   Adds JT's get3DMovement14Directions(bool ignoreYAxis) function...needs tested
/// 1.6     Line 461 - Debounced the escape button for DetectBackPressedByAnyPlayer()
/// 1.5     Uses singleton design, revision of the GetAs8DirectionLeft and RightThumbStick().
/// 1.4     Adds indexing of Gamepad by int or by PlayerIndex. <device><current || previous><[index]>.
/// 1.3     Adds Xbox input, Cole Stoltzfus's inspiration for individual gamepads, mouse is limited to Windows compilation target.
/// 1.2     Adds Mouse input and mouseDelta for 3D cameraP1 usage.
/// 1.1     4-way & 8-way arrow key creates vector2D for 2D games.
/// 1.0     Keyboard input only.
/// 
/// Works with MS Visual Studio 10.0, XNA Framework 4.0
/// -->
namespace InputHandler
{
    /// <summary>
    /// A singleton wrapper class for listening for certain key presses. 
    /// 
    /// To initialize, write the following in your game's constructor:
    /// ClaudyInput ci = ClaudyInput.Instance;
    /// 
    /// Stores the current and previous keyboard and mouse and GamePad states 
    /// so that the Game class doesn't need to.
    /// </summary>
    public sealed class Input
    {
        //Singleton with public field design: See http://www.dotnetperls.com/singleton
        public static readonly Input Instance = new Input(); 
        ////////////////////////////////////////////////////////////////

        // Note to self: members are only public if explicitly, individually defined as public.
        // Development choice: there are overloads for empty player index defaulting to controller one.
        // Defaulting is faster than index resolving by a tiny bit. Also, update is slightly slower
        // due to assignments (likely negligible impact). 
        KeyboardState keyboardCurrent;
        KeyboardState keyboardPrevious;

        #if WINDOWS
        MouseState mouseCurrent;
        MouseState mousePrevious;
        Vector2 mouseDelta;
        #endif

        /// <summary>
        /// Indexed reference to each player's gamepad. Player 1 is [1], P2 is [2] etc... [0] is null.
        /// </summary>
        GamePadState[] gamepadByID;
        GamePadState[] previousGamepadByID;
        private GamePadState gamePadCurrent1;
        private GamePadState gamePadPrevious1;
        private GamePadState gamePadCurrent2;
        private GamePadState gamePadPrevious2;
        private GamePadState gamePadCurrent3;
        private GamePadState gamePadPrevious3;
        private GamePadState gamePadCurrent4;
        private GamePadState gamePadPrevious4;
        

        public KeyboardState KeyboardCurrent
        {
            get { return keyboardCurrent; }
            //protected set { }
        }
        public KeyboardState KeyboardPrevious
        {
            get { return keyboardPrevious; }
            //protected set { }
        }

        #if WINDOWS
        public MouseState MouseCurrent
        {
            get { return mouseCurrent; }
            set { mouseCurrent = value; }
        }
        public MouseState MousePrevious
        {
            get { return mousePrevious; }
            set { mousePrevious = value; }
        }
        #endif
        #region GamePad Property Declarations
        /// <summary>
        /// Indexed reference to each player's gamepad. Player 1 is [1], P2 is [2] etc... [0] is null.
        /// </summary>
        /// <remarks>int index = (int)p_ + 1; will come in handy where p_ is of type PlayerIndex.</remarks>
        public GamePadState[] GamepadByID
        {
            get { return gamepadByID; }
            //protected set { }
        }
        /// <summary>
        /// Indexed reference to each player's gamepad. Player 1 is [1], P2 is [2] etc... [0] is null.
        /// </summary>
        /// <remarks>int index = (int)p_ + 1; will come in handy where p_ is of type PlayerIndex.</remarks>
        public GamePadState[] PreviousGamepadByID
        {
            get { return previousGamepadByID; }
            //protected set { }
        }

        public GamePadState GamePadCurrent1
        {
            get { return gamePadCurrent1; }
            //protected set { }
        }
        public GamePadState GamePadPrevious1
        {
            get { return gamePadPrevious1; }
            //protected set { }
        }
        public GamePadState GamePadCurrent2
        {
            get { return gamePadCurrent2; }
            //protected set { }
        }
        public GamePadState GamePadPrevious2
        {
            get { return gamePadPrevious2; }
            //protected set { }
        }
        public GamePadState GamePadCurrent3
        {
            get { return gamePadCurrent3; }
            //protected set { }
        }
        public GamePadState GamePadPrevious3
        {
            get { return gamePadPrevious3; }
            //protected set { }
        }
        public GamePadState GamePadCurrent4
        {
            get { return gamePadCurrent4; }
            //protected set { }
        }
        public GamePadState GamePadPrevious4
        {
            get { return gamePadPrevious4; }
            //protected set { }
        }
        #endregion

        // CTOR
        private Input()
        {
            keyboardPrevious = keyboardCurrent; // Initialize previous as "no action occurring".
            keyboardCurrent = Keyboard.GetState();

            #if WINDOWS
            mousePrevious = mouseCurrent;       // Initialize previous as "no action occurring".
            mouseCurrent = Mouse.GetState();
            mouseDelta = Vector2.Zero;
            #endif

            gamePadPrevious1 = gamePadCurrent1; // Initialize previous as "no action occurring".
            gamePadCurrent1 = GamePad.GetState(PlayerIndex.One);

            gamePadPrevious2 = gamePadCurrent2; // Initialize previous as "no action occurring".
            gamePadCurrent2 = GamePad.GetState(PlayerIndex.Two);
            
            gamePadPrevious3 = gamePadCurrent3; // Initialize previous as "no action occurring".
            gamePadCurrent3 = GamePad.GetState(PlayerIndex.Three);

            gamePadPrevious4 = gamePadCurrent4; // Initialize previous as "no action occurring".
            gamePadCurrent4 = GamePad.GetState(PlayerIndex.Four);

            gamepadByID = new GamePadState[5];
            gamepadByID[0] = new GamePadState();
            gamepadByID[1] = gamePadCurrent1;
            gamepadByID[2] = gamePadCurrent2;
            gamepadByID[3] = gamePadCurrent3;
            gamepadByID[4] = gamePadCurrent4;
            previousGamepadByID = new GamePadState[5];
            previousGamepadByID[0] = new GamePadState();
            previousGamepadByID[1] = gamePadPrevious1;
            previousGamepadByID[2] = gamePadPrevious2;
            previousGamepadByID[3] = gamePadPrevious3;
            previousGamepadByID[4] = gamePadPrevious4;

        }

        /// <summary>
        /// Updates the input state. PROTIP: CALL THIS ONCE ONLY IN MAIN GAME LOOP.
        /// </summary>
        public void Update()
        {
            keyboardPrevious = keyboardCurrent;
            keyboardCurrent = Keyboard.GetState();

            #if WINDOWS
            mousePrevious = mouseCurrent;
            mouseCurrent = Mouse.GetState();
            mouseDelta = new Vector2(
                mouseCurrent.X - mousePrevious.X,
                mouseCurrent.Y - mousePrevious.Y);
            #endif

            gamePadPrevious1 = gamePadCurrent1; // Initialize previous as "no action occurring".
            gamePadCurrent1 = GamePad.GetState(PlayerIndex.One);

            gamePadPrevious2 = gamePadCurrent2; // Initialize previous as "no action occurring".
            gamePadCurrent2 = GamePad.GetState(PlayerIndex.Two);

            gamePadPrevious3 = gamePadCurrent3; // Initialize previous as "no action occurring".
            gamePadCurrent3 = GamePad.GetState(PlayerIndex.Three);

            gamePadPrevious4 = gamePadCurrent4; // Initialize previous as "no action occurring".
            gamePadCurrent4 = GamePad.GetState(PlayerIndex.Four);

            gamepadByID[1] = gamePadCurrent1;
            gamepadByID[2] = gamePadCurrent2;
            gamepadByID[3] = gamePadCurrent3;
            gamepadByID[4] = gamePadCurrent4;
            previousGamepadByID[1] = gamePadPrevious1;
            previousGamepadByID[2] = gamePadPrevious2;
            previousGamepadByID[3] = gamePadPrevious3;
            previousGamepadByID[4] = gamePadPrevious4;
        }

        /// <summary>
        /// Helper function to convert PlayerIndex into a player 1 2 3 or 4.
        /// </summary>
        /// <param name="p_"></param>
        /// <returns></returns>
        public int toInt(PlayerIndex p_)
        {
            return (int)p_ + 1;
        }

        #region Keyboard controls
        /// <summary>
        /// Returns true if that key is currently being pressed. Repeat results of true are likely.
        /// </summary>
        /// <param name="k_">The key to be queried.</param>
        /// <param name="current_">The current keyboard state.</param>
        /// <returns>Returns true if the key was just pressed.</returns>
        public bool isPressed(Keys k_)
        {
            return keyboardCurrent.IsKeyDown(k_);
        }

        /// <summary>
        /// Detects the down stroke of a key. Returns true if the key was just pressed. Will not repeat true.
        /// </summary>
        /// <param name="k_">The key to be queried.</param>
        /// <param name="current_">The current keyboard state.</param>
        /// <param name="previous_">The previous keyboard state.</param>
        /// <returns>Returns true if the key was just pressed.</returns>
        public bool isFirstPress(Keys k_)
        {
            return (keyboardCurrent.IsKeyDown(k_) && keyboardPrevious.IsKeyUp(k_));
        }
        
        /// <summary>
        /// Returns true upon the release of key k_. "Debounced" detection. Will not repeat true.
        /// </summary>
        /// <param name="k_">The key to be queried.</param>
        /// <param name="current_">The current keyboard state.</param>
        /// <param name="previous_">The previous keyboard state.</param>
        /// <returns>Returns true if the key was just released.</returns>
        public bool isReleased(Keys k_)
        {
            return (keyboardCurrent.IsKeyUp(k_) && keyboardPrevious.IsKeyDown(k_));
        }

        /// <summary>
        /// Returns the status of the arrow keys in the form of a normalized Vector2 which can
        /// be used for multiplication of a scalar or vector velocity. Think of this in terms
        /// of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// </summary>
        /// <returns>Vector2 of direction.</returns>
        public Vector2 getArrowKeys8Directions()
        {
            Vector2 movement = new Vector2(0f, 0f);
            if (keyboardCurrent.IsKeyDown(Keys.Up))
                movement.Y--;
            if (keyboardCurrent.IsKeyDown(Keys.Down))
                movement.Y++;
            if (keyboardCurrent.IsKeyDown(Keys.Left))
                movement.X--;
            if (keyboardCurrent.IsKeyDown(Keys.Right))
                movement.X++;
            if (movement != Vector2.Zero) movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns only one direction in form of a Vector2 which can be used for multiplication 
        /// of a scalar or vector velocity. Think of this in terms of a direction filter.
        /// 4 directions possible, no overlapping, canceling out possible.
        /// This is better than an if-else logic method because it doesn't give logical priority
        /// to a single direction. However, this is implemented to give logical priority to the
        /// up-down axis.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// </summary>
        /// <returns>A single direction, one element of the vector shall always be zero.</returns>
        public Vector2 getArrowKeys4Directions()
        {
            Vector2 movement = Vector2.Zero;
            if (keyboardCurrent.IsKeyDown(Keys.Up))
                movement.Y--;
            if (keyboardCurrent.IsKeyDown(Keys.Down))
                movement.Y++;
            if (movement.Y != 0) return movement;
            // Vertical axis input not utilized, check horizontal input.
            if (keyboardCurrent.IsKeyDown(Keys.Left))
                movement.X--;
            if (keyboardCurrent.IsKeyDown(Keys.Right))
                movement.X++;
            return movement; // Return will logically need to occur regardless.
        }


        /// <summary>
        /// New input function that takes WASD and returns the movement direction
        /// uses a Vector3 for 3D movement.
        /// Added by JT Broad in the GrannyCrosswalk project.
        /// There exists an overload that will do this on an Xbox Controller
        /// </summary>
        /// <param name="ignoreYAxis">False, unless player shall not fly in which case set to True.</param>
        public Vector3 get3DMovement14Directions(bool ignoreYAxis)
        {
            Vector3 movement = new Vector3(0f, 0f, 0f);
            if (keyboardCurrent.IsKeyDown(Keys.W))
                movement.Z--;
            if (keyboardCurrent.IsKeyDown(Keys.S))
                movement.Z++;
            if (keyboardCurrent.IsKeyDown(Keys.A))
                movement.X--;
            if (keyboardCurrent.IsKeyDown(Keys.D))
                movement.X++;
            if (movement != Vector3.Zero) movement.Normalize();
            if (!ignoreYAxis)
            {
                if (keyboardCurrent.IsKeyDown(Keys.Z)) movement.Y++;
                if (keyboardCurrent.IsKeyDown(Keys.X)) movement.Y--;
            }
            return movement;
        }
        #endregion

        #if WINDOWS
        #region Mouse controls
        /// <summary>
        /// Debounced version of Left Mouse Button is clicked.
        /// </summary>
        public bool isLeftMouseButtonReleased()
        {
            return (mouseCurrent.LeftButton == ButtonState.Released && mousePrevious.LeftButton == ButtonState.Pressed);
        }
        public bool isMiddleMouseButtonReleased()
        {
            return (mouseCurrent.MiddleButton == ButtonState.Released && mousePrevious.MiddleButton == ButtonState.Pressed);
        }
        public bool isRightMouseButtonReleased()
        {
            return (mouseCurrent.RightButton == ButtonState.Released && mousePrevious.RightButton == ButtonState.Pressed);
        }


        /// <summary>
        /// For those who need a Point structure instead of using the public accessor.
        /// Use this sparingly.
        /// </summary>
        /// <returns></returns>
        public Point getLocationOfMouse()
        {
            return new Point(mouseCurrent.X, mouseCurrent.Y);
        }
        public Vector2 getMouseDelta()
        {
            return mouseDelta;
        }
        #endregion
        #endif

        #region Xbox 360 GamePad controls
        public bool isConnected(PlayerIndex p_)
        {
            return gamepadByID[((int)p_ + 1)].IsConnected;
        }
        public bool isConnected(int playerIndex)
        {
            if(playerIndex <= 0 || playerIndex > 4) throw new ArgumentOutOfRangeException("playerIndex", "A function argument was not between 1 and 4. Only players 1 through 4 shall be accepted.");
            return gamepadByID[playerIndex].IsConnected;
        }

        /// <summary>
        /// Returns true if that button is currently being pressed. Repeat results of true are likely.
        /// Defaults to the first player if PlayerIndex is not specified.
        /// </summary>
        /// <returns>Returns true if the button was just pressed.</returns>
        public bool isPressed(Buttons b_)
        {
            return gamePadCurrent1.IsButtonDown(b_); // Default PlayerIndex is first player.
        }
        /// <summary>
        /// Returns true if that button is currently being pressed. Repeat results of true are likely.
        /// </summary>
        /// <returns>Returns true if the button was just pressed.</returns>
        public bool isPressed(Buttons b_, PlayerIndex p_)
        {
            switch(p_)
            {
                case PlayerIndex.One:
                    return gamePadCurrent1.IsButtonDown(b_);
                case PlayerIndex.Two:
                    return gamePadCurrent2.IsButtonDown(b_);
                case PlayerIndex.Three:
                    return gamePadCurrent3.IsButtonDown(b_);
                case PlayerIndex.Four:
                    return gamePadCurrent4.IsButtonDown(b_);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Detects the down stroke of a button press. Returns true if the button was just pressed. Will not repeat true. 
        /// Defaults to the first player if PlayerIndex is not specified.
        /// </summary>
        /// <param name="b_">The button to be queried.</param>
        /// <returns>True if the button was just pressed.</returns>
        public bool isFirstPress(Buttons b_)
        {
            return GamePadCurrent1.IsButtonDown(b_) && gamePadPrevious1.IsButtonUp(b_);
        }

        /// <summary>
        /// Detects the down stroke of a button press. Returns true if the button was just pressed. Will not repeat true.
        /// </summary>
        /// <param name="b_">The button to be queried.</param>
        /// <returns>Returns true if the button was just pressed.</returns>
        public bool isFirstPress(Buttons b_, PlayerIndex p_)
        {
            switch(p_)
            {
                case PlayerIndex.One:
                    return gamePadCurrent1.IsButtonDown(b_) && gamePadPrevious1.IsButtonUp(b_);
                case PlayerIndex.Two:
                    return gamePadCurrent2.IsButtonDown(b_) && gamePadPrevious2.IsButtonUp(b_);
                case PlayerIndex.Three:
                    return gamePadCurrent3.IsButtonDown(b_) && gamePadPrevious3.IsButtonUp(b_);
                case PlayerIndex.Four:
                    return gamePadCurrent4.IsButtonDown(b_) && gamePadPrevious4.IsButtonUp(b_);
                default:
                    return false;
            }
        }

        /// <summary>
        /// "Debounced" detection. Returns true upon the release of button b_. Will not repeat true.
        /// Defaults to first player if PlayerIndex is not specified.
        /// </summary>
        /// <param name="b_">The button to be queried.</param>
        /// <returns>Returns true if the key was just released.</returns>
        public bool isReleased(Buttons b_)
        {
            return (gamePadCurrent1.IsButtonUp(b_) && gamePadPrevious1.IsButtonDown(b_));
        }

        /// <summary>
        /// "Debounced" detection. Returns true upon the release of button b_. Will not repeat true.
        /// </summary>
        /// <param name="b_">The button to be queried.</param>
        /// <returns>Returns true if the key was just released.</returns>
        public bool isReleased(Buttons b_, PlayerIndex p_)
        {
            switch (p_)
            {
                case PlayerIndex.One:
                    return gamePadCurrent1.IsButtonUp(b_) && gamePadPrevious1.IsButtonDown(b_);
                case PlayerIndex.Two:
                    return gamePadCurrent2.IsButtonUp(b_) && gamePadPrevious2.IsButtonDown(b_);
                case PlayerIndex.Three:
                    return gamePadCurrent3.IsButtonUp(b_) && gamePadPrevious3.IsButtonDown(b_);
                case PlayerIndex.Four:
                    return gamePadCurrent4.IsButtonUp(b_) && gamePadPrevious4.IsButtonDown(b_);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns True if the game should exit.
        /// </summary>
        /// <returns></returns>
        public bool DetectBackPressedByAnyPlayer()
        {
            return (isFirstPress(Keys.Escape) ||
                isFirstPress(Buttons.Back, PlayerIndex.One) ||
                isFirstPress(Buttons.Back, PlayerIndex.Two) ||
                isFirstPress(Buttons.Back, PlayerIndex.Three) ||
                isFirstPress(Buttons.Back, PlayerIndex.Four));
        }

        /// <summary>
        /// Returns True if any controller is still connected.
        /// Returns False if all controllers are disconnected.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyControllersPluggedIn()
        {
            return (
                gamePadCurrent1.IsConnected ||
                gamePadCurrent2.IsConnected ||
                gamePadCurrent3.IsConnected ||
                gamePadCurrent4.IsConnected
                );
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionLeftThumbStick()
        {
            Vector2 movement = new Vector2(0f, 0f);
            if (Math.Abs(gamePadCurrent1.ThumbSticks.Left.X) < .99f && gamePadCurrent1.ThumbSticks.Left.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamePadCurrent1.ThumbSticks.Left.X) < .99f && gamePadCurrent1.ThumbSticks.Left.Y > 0.0f)
                movement.Y++;
            if (gamePadCurrent1.ThumbSticks.Left.X < 0.0f && Math.Abs(gamePadCurrent1.ThumbSticks.Left.Y) < .99f)
                movement.X--;
            if (gamePadCurrent1.ThumbSticks.Left.X > 0.0f && Math.Abs(gamePadCurrent1.ThumbSticks.Left.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero) 
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionLeftThumbStick(PlayerIndex p_)
        {
            int index = (int)p_ + 1;
            Vector2 movement = new Vector2(0f, 0f);

            if (Math.Abs(gamepadByID[index].ThumbSticks.Left.X) < .99f && gamepadByID[index].ThumbSticks.Left.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamepadByID[index].ThumbSticks.Left.X) < .99f && gamepadByID[index].ThumbSticks.Left.Y > 0.0f)
                movement.Y++;
            if (gamepadByID[index].ThumbSticks.Left.X < 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Left.Y) < .99f)
                movement.X--;
            if (gamepadByID[index].ThumbSticks.Left.X > 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Left.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero)
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionLeftThumbStick(int index)
        {
            if (index <= 0 || index > 4) throw new ArgumentOutOfRangeException("index", "A function argument was not between 1 and 4. Only players 1 through 4 shall be accepted.");
            Vector2 movement = new Vector2(0f, 0f);

            if (Math.Abs(gamepadByID[index].ThumbSticks.Left.X) < .99f && gamepadByID[index].ThumbSticks.Left.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamepadByID[index].ThumbSticks.Left.X) < .99f && gamepadByID[index].ThumbSticks.Left.Y > 0.0f)
                movement.Y++;
            if (gamepadByID[index].ThumbSticks.Left.X < 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Left.Y) < .99f)
                movement.X--;
            if (gamepadByID[index].ThumbSticks.Left.X > 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Left.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero)
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionRightThumbStick()
        {
            Vector2 movement = new Vector2(0f, 0f);
            if (Math.Abs(gamePadCurrent1.ThumbSticks.Right.X) < .99f && gamePadCurrent1.ThumbSticks.Right.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamePadCurrent1.ThumbSticks.Right.X) < .99f && gamePadCurrent1.ThumbSticks.Right.Y > 0.0f)
                movement.Y++;
            if (gamePadCurrent1.ThumbSticks.Right.X < 0.0f && Math.Abs(gamePadCurrent1.ThumbSticks.Right.Y) < .99f)
                movement.X--;
            if (gamePadCurrent1.ThumbSticks.Right.X > 0.0f && Math.Abs(gamePadCurrent1.ThumbSticks.Right.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero)
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionRightThumbStick(PlayerIndex p_)
        {
            int index = (int)p_ + 1;
            Vector2 movement = new Vector2(0f, 0f);

            if (Math.Abs(gamepadByID[index].ThumbSticks.Right.X) < .99f && gamepadByID[index].ThumbSticks.Right.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamepadByID[index].ThumbSticks.Right.X) < .99f && gamepadByID[index].ThumbSticks.Right.Y > 0.0f)
                movement.Y++;
            if (gamepadByID[index].ThumbSticks.Right.X < 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Right.Y) < .99f)
                movement.X--;
            if (gamepadByID[index].ThumbSticks.Right.X > 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Right.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero)
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// Returns the status of the Left Thumbstick in the form of a normalized, 8-direction Vector2.
        /// Think of this in terms of a direction filter. 8 directions of freedom.
        /// 
        /// Adapted from the TrackAndEvade1 example code.
        /// Defaults to first player if no PlayerIndex is specified.
        /// </summary>
        /// <returns>Vector2 of direction. X and Y values possibilities: { 0f, +-0.7071f, +-1f }</returns>
        public Vector2 GetAs8DirectionRightThumbStick(int index)
        {
            if (index <= 0 || index > 4) throw new ArgumentOutOfRangeException("index", "A function argument was not between 1 and 4. Only players 1 through 4 shall be accepted.");
            Vector2 movement = new Vector2(0f, 0f);

            if (Math.Abs(gamepadByID[index].ThumbSticks.Right.X) < .99f && gamepadByID[index].ThumbSticks.Right.Y < 0.0f)
                movement.Y--;
            if (Math.Abs(gamepadByID[index].ThumbSticks.Right.X) < .99f && gamepadByID[index].ThumbSticks.Right.Y > 0.0f)
                movement.Y++;
            if (gamepadByID[index].ThumbSticks.Right.X < 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Right.Y) < .99f)
                movement.X--;
            if (gamepadByID[index].ThumbSticks.Right.X > 0.0f && Math.Abs(gamepadByID[index].ThumbSticks.Right.Y) < .99f)
                movement.X++;
            if (movement != Vector2.Zero)
                movement.Normalize();
            return movement;
        }

        /// <summary>
        /// New input function that gets the Xbox 360 gamepad thumbsticks
        /// and generates a Vector3 for 3D movement.
        /// Adapted from GrannyCrosswalk code.
        /// </summary>
        /// <param name="ignoreYAxis">False, unless player shall not fly in which case set to True.</param>
        public Vector3 get3DMovement14Directions(bool ignoreYAxis, /*bool invertY or X,*/ PlayerIndex p_)
        {
            switch (p_)
            {
                case PlayerIndex.Four:
                    return get3DMovement14Directions(ignoreYAxis, 4);
                case PlayerIndex.One:
                    return get3DMovement14Directions(ignoreYAxis, 1);
                case PlayerIndex.Three:
                    return get3DMovement14Directions(ignoreYAxis, 3);
                case PlayerIndex.Two:
                    return get3DMovement14Directions(ignoreYAxis, 2);
                default: break;
            }
            return Vector3.Zero;
        }

        /// <summary>
        /// New input function that gets the Xbox 360 gamepad thumbsticks
        /// and generates a Vector3 for 3D movement.
        /// Adapted from GrannyCrosswalk code.
        /// </summary>
        /// <param name="ignoreYAxis">False, unless player shall not fly in which case set to True.</param>
        public Vector3 get3DMovement14Directions(bool ignoreYAxis, /*bool invertY or X,*/ int index)
        {
            if (!gamepadByID[index].IsConnected)
                return Vector3.Zero;

            Vector3 movement = Vector3.Zero;
            movement.X -= gamepadByID[index].ThumbSticks.Left.X;
            if (!ignoreYAxis)
            {
                movement.Y = gamepadByID[index].ThumbSticks.Right.Y;
            }
            movement.Z = gamepadByID[index].ThumbSticks.Left.Y;

            if (movement != Vector3.Zero) movement.Normalize();

            return movement;
        }

        #endregion
    }
}