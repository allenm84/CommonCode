using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Common.References
{
  public enum ButtonState
  {
    Released,
    Pressed
  }

  [Flags]
  internal enum ButtonValues : ushort
  {
    A = 4096,
    B = 8192,
    Back = 32,
    Down = 2,
    Left = 4,
    LeftShoulder = 256,
    LeftThumb = 64,
    Right = 8,
    RightShoulder = 512,
    RightThumb = 128,
    Start = 16,
    Up = 1,
    X = 16384,
    Y = 32768,
    BigButton = 2048
  }

  internal enum ErrorCodes : uint
  {
    Success,
    Pending = 997u,
    NotConnected = 1167u,
    Empty = 4306u,
    Busy = 170u,
    AccessDenied = 5u,
    AlreadyExists = 183u,
    D3DERR_WRONGTEXTUREFORMAT = 2289436696u,
    D3DERR_TOOMANYOPERATIONS = 2289436701u,
    D3DERR_DRIVERINTERNALERROR = 2289436711u,
    D3DERR_NOTFOUND = 2289436774u,
    D3DERR_MOREDATA,
    D3DERR_DEVICELOST,
    D3DERR_DEVICENOTRESET,
    D3DERR_NOTAVAILABLE = 2289436784u,
    D3DERR_OUTOFVIDEOMEMORY = 2289435004u,
    D3DERR_INVALIDCALL = 2289436786u,
    XACTENGINE_E_ALREADYINITIALIZED = 2328297473u,
    XACTENGINE_E_NOTINITIALIZED,
    XACTENGINE_E_EXPIRED,
    XACTENGINE_E_NONOTIFICATIONCALLBACK,
    XACTENGINE_E_NOTIFICATIONREGISTERED,
    XACTENGINE_E_INVALIDUSAGE,
    XACTENGINE_E_INVALIDDATA,
    XACTENGINE_E_INSTANCELIMITFAILTOPLAY,
    XACTENGINE_E_NOGLOBALSETTINGS,
    XACTENGINE_E_INVALIDVARIABLEINDEX,
    XACTENGINE_E_INVALIDCATEGORY,
    XACTENGINE_E_INVALIDCUEINDEX,
    XACTENGINE_E_INVALIDWAVEINDEX,
    XACTENGINE_E_INVALIDTRACKINDEX,
    XACTENGINE_E_INVALIDSOUNDOFFSETORINDEX,
    XACTENGINE_E_READFILE,
    XACTENGINE_E_UNKNOWNEVENT,
    XACTENGINE_E_INCALLBACK,
    XACTENGINE_E_NOWAVEBANK,
    XACTENGINE_E_SELECTVARIATION,
    XACTENGINE_E_MULTIPLEAUDITIONENGINES,
    XACTENGINE_E_WAVEBANKNOTPREPARED,
    XACTENGINE_E_NORENDERER,
    XACTENGINE_E_INVALIDENTRYCOUNT,
    XACTENGINE_E_SEEKTIMEBEYONDCUEEND,
    XACTENGINE_E_AUDITION_WRITEFILE = 2328297729u,
    XACTENGINE_E_AUDITION_NOSOUNDBANK,
    XACTENGINE_E_AUDITION_INVALIDRPCINDEX,
    XACTENGINE_E_AUDITION_MISSINGDATA,
    XACTENGINE_E_AUDITION_UNKNOWNCOMMAND,
    XACTENGINE_E_AUDITION_INVALIDDSPINDEX,
    XACTENGINE_E_AUDITION_MISSINGWAVE,
    XACTENGINE_E_AUDITION_CREATEDIRECTORYFAILED,
    XACTENGINE_E_AUDITION_INVALIDSESSION,
    ZDKSYSTEM_E_AUDIO_INSTANCELIMIT = 2343370753u,
    ZDKSYSTEM_E_AUDIO_INVALIDSTATE,
    ZDKSYSTEM_E_AUDIO_INVALIDDATA,
    CAPTURE_ENGINE_E_DEVICEGONE = 2364407809u,
    VFW_E_NO_AUDIO_HARDWARE = 2147746390u,
    E_INVALIDARG = 2147942487u,
    E_FAIL = 2147500037u,
    E_ABORT = 2147500036u,
    E_ACCESSDENIED = 2147942405u,
    E_NOTIMPL = 2147500033u,
    E_OUTOFMEMORY = 2147942414u,
    STRSAFE_E_INSUFFICIENT_BUFFER = 2147942522u,
    REGDB_E_CLASSNOTREG = 2147746132u,
    ERROR_SHARING_VIOLATION = 2147942432u
  }

  public static class GamePad
  {
    private static bool[] _disconnected = new bool[4];
    private static long[] _lastReadTime = new long[4];

    public static GamePadState GetState(PlayerIndex playerIndex)
    {
      return GamePad.GetState(playerIndex, GamePadDeadZone.IndependentAxes);
    }

    public static GamePadState GetState(PlayerIndex playerIndex, GamePadDeadZone deadZoneMode)
    {
      XINPUT_STATE xINPUT_STATE = default(XINPUT_STATE);
      ErrorCodes errorCodes;
      if (GamePad.ThrottleDisconnectedRetries(playerIndex))
      {
        errorCodes = ErrorCodes.NotConnected;
      }
      else
      {
        errorCodes = UnsafeNativeMethods.GetState(playerIndex, out xINPUT_STATE);
        GamePad.ResetThrottleState(playerIndex, errorCodes);
      }
      if (errorCodes != ErrorCodes.Success && errorCodes != ErrorCodes.NotConnected)
      {
        throw new InvalidOperationException("InvalidController");
      }
      return new GamePadState(ref xINPUT_STATE, errorCodes, deadZoneMode);
    }

    public static GamePadCapabilities GetCapabilities(PlayerIndex playerIndex)
    {
      XINPUT_CAPABILITIES xINPUT_CAPABILITIES = default(XINPUT_CAPABILITIES);
      ErrorCodes errorCodes;
      if (GamePad.ThrottleDisconnectedRetries(playerIndex))
      {
        errorCodes = ErrorCodes.NotConnected;
      }
      else
      {
        errorCodes = UnsafeNativeMethods.GetCaps(playerIndex, 1u, out xINPUT_CAPABILITIES);
        GamePad.ResetThrottleState(playerIndex, errorCodes);
      }
      if (errorCodes != ErrorCodes.Success && errorCodes != ErrorCodes.NotConnected)
      {
        throw new InvalidOperationException("InvalidController");
      }
      return new GamePadCapabilities(ref xINPUT_CAPABILITIES, errorCodes);
    }

    public static bool SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
    {
      XINPUT_VIBRATION xINPUT_VIBRATION;
      xINPUT_VIBRATION.LeftMotorSpeed = (short)(leftMotor * 65535f);
      xINPUT_VIBRATION.RightMotorSpeed = (short)(rightMotor * 65535f);
      ErrorCodes errorCodes;
      if (GamePad.ThrottleDisconnectedRetries(playerIndex))
      {
        errorCodes = ErrorCodes.NotConnected;
      }
      else
      {
        errorCodes = UnsafeNativeMethods.SetState(playerIndex, ref xINPUT_VIBRATION);
        GamePad.ResetThrottleState(playerIndex, errorCodes);
      }
      if (errorCodes == ErrorCodes.Success)
      {
        return true;
      }
      if (errorCodes != ErrorCodes.Success && errorCodes != ErrorCodes.NotConnected && errorCodes != ErrorCodes.Busy)
      {
        throw new InvalidOperationException("InvalidController");
      }
      return false;
    }

    private static bool ThrottleDisconnectedRetries(PlayerIndex playerIndex)
    {
      if (playerIndex < PlayerIndex.One || playerIndex > PlayerIndex.Four)
      {
        return false;
      }
      if (!GamePad._disconnected[(int)playerIndex])
      {
        return false;
      }
      long timestamp = Stopwatch.GetTimestamp();
      for (int i = 0; i < 4; i++)
      {
        if (GamePad._disconnected[i])
        {
          long num = timestamp - GamePad._lastReadTime[i];
          long num2 = Stopwatch.Frequency;
          if (i != (int)playerIndex)
          {
            num2 /= 4L;
          }
          if (num >= 0L && num <= num2)
          {
            return true;
          }
        }
      }
      return false;
    }

    private static void ResetThrottleState(PlayerIndex playerIndex, ErrorCodes result)
    {
      if (playerIndex < PlayerIndex.One || playerIndex > PlayerIndex.Four)
      {
        return;
      }
      if (result == ErrorCodes.NotConnected)
      {
        GamePad._disconnected[(int)playerIndex] = true;
        GamePad._lastReadTime[(int)playerIndex] = Stopwatch.GetTimestamp();
        return;
      }
      GamePad._disconnected[(int)playerIndex] = false;
    }
  }

  public struct GamePadButtons
  {
    internal ButtonState _a;
    internal ButtonState _b;
    internal ButtonState _x;
    internal ButtonState _y;
    internal ButtonState _leftStick;
    internal ButtonState _rightStick;
    internal ButtonState _leftShoulder;
    internal ButtonState _rightShoulder;
    internal ButtonState _back;
    internal ButtonState _start;
    internal ButtonState _bigButton;

    public ButtonState A
    {
      get
      {
        return this._a;
      }
    }
    public ButtonState B
    {
      get
      {
        return this._b;
      }
    }

    public ButtonState Back
    {
      get
      {
        return this._back;
      }
    }

    public ButtonState X
    {
      get
      {
        return this._x;
      }
    }

    public ButtonState Y
    {
      get
      {
        return this._y;
      }
    }

    public ButtonState Start
    {
      get
      {
        return this._start;
      }
    }

    public ButtonState LeftShoulder
    {
      get
      {
        return this._leftShoulder;
      }
    }

    public ButtonState LeftStick
    {
      get
      {
        return this._leftStick;
      }
    }

    public ButtonState RightShoulder
    {
      get
      {
        return this._rightShoulder;
      }
    }

    public ButtonState RightStick
    {
      get
      {
        return this._rightStick;
      }
    }

    public ButtonState BigButton
    {
      get
      {
        return this._bigButton;
      }
    }

    public GamePadButtons(XButtons buttons)
    {
      this._a = (((buttons & XButtons.A) == XButtons.A) ? ButtonState.Pressed : ButtonState.Released);
      this._b = (((buttons & XButtons.B) == XButtons.B) ? ButtonState.Pressed : ButtonState.Released);
      this._x = (((buttons & XButtons.X) == XButtons.X) ? ButtonState.Pressed : ButtonState.Released);
      this._y = (((buttons & XButtons.Y) == XButtons.Y) ? ButtonState.Pressed : ButtonState.Released);
      this._start = (((buttons & XButtons.Start) == XButtons.Start) ? ButtonState.Pressed : ButtonState.Released);
      this._back = (((buttons & XButtons.Back) == XButtons.Back) ? ButtonState.Pressed : ButtonState.Released);
      this._leftStick = (((buttons & XButtons.LeftStick) == XButtons.LeftStick) ? ButtonState.Pressed : ButtonState.Released);
      this._rightStick = (((buttons & XButtons.RightStick) == XButtons.RightStick) ? ButtonState.Pressed : ButtonState.Released);
      this._leftShoulder = (((buttons & XButtons.LeftShoulder) == XButtons.LeftShoulder) ? ButtonState.Pressed : ButtonState.Released);
      this._rightShoulder = (((buttons & XButtons.RightShoulder) == XButtons.RightShoulder) ? ButtonState.Pressed : ButtonState.Released);
      this._bigButton = (((buttons & XButtons.BigButton) == XButtons.BigButton) ? ButtonState.Pressed : ButtonState.Released);
    }

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != base.GetType()) && this == (GamePadButtons)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      string text = string.Empty;
      if (this._a == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "A";
      }
      if (this._b == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "B";
      }
      if (this._x == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "X";
      }
      if (this._y == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Y";
      }
      if (this._leftShoulder == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "LeftShoulder";
      }
      if (this._rightShoulder == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "RightShoulder";
      }
      if (this._leftStick == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "LeftStick";
      }
      if (this._rightStick == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "RightStick";
      }
      if (this._start == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Start";
      }
      if (this._back == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Back";
      }
      if (this._bigButton == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "BigButton";
      }
      if (text.Length == 0)
      {
        text = "None";
      }
      return string.Format(CultureInfo.CurrentCulture, "{{Buttons:{0}}}", new object[]
      {
        text
      });
    }

    public static bool operator ==(GamePadButtons left, GamePadButtons right)
    {
      return left._a == right._a && left._b == right._b && left._x == right._x && left._y == right._y && left._leftShoulder == right._leftShoulder && left._leftStick == right._leftStick && left._rightShoulder == right._rightShoulder && left._rightStick == right._rightStick && left._back == right._back && left._start == right._start && left._bigButton == right._bigButton;
    }

    public static bool operator !=(GamePadButtons left, GamePadButtons right)
    {
      return left._a != right._a || left._b != right._b || left._x != right._x || left._y != right._y || left._leftShoulder != right._leftShoulder || left._leftStick != right._leftStick || left._rightShoulder != right._rightShoulder || left._rightStick != right._rightStick || left._back != right._back || left._start != right._start || left._bigButton != right._bigButton;
    }
  }

  public struct GamePadCapabilities
  {
    private bool _connected;
    private XINPUT_CAPABILITIES _caps;

    public GamePadType GamePadType
    {
      get
      {
        if (this._caps.Type == 3)
        {
          return (GamePadType)((int)this._caps.Type << 8 | (int)this._caps.SubType);
        }
        return (GamePadType)this._caps.SubType;
      }
    }

    public bool IsConnected
    {
      get
      {
        return this._connected;
      }
    }

    public bool HasAButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.A) != 0;
      }
    }

    public bool HasBackButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Back) != 0;
      }
    }

    public bool HasBButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.B) != 0;
      }
    }

    public bool HasDPadDownButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Down) != 0;
      }
    }

    public bool HasDPadLeftButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Left) != 0;
      }
    }

    public bool HasDPadRightButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Right) != 0;
      }
    }

    public bool HasDPadUpButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Up) != 0;
      }
    }

    public bool HasLeftShoulderButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.LeftShoulder) != 0;
      }
    }

    public bool HasLeftStickButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.LeftThumb) != 0;
      }
    }

    public bool HasRightShoulderButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.RightShoulder) != 0;
      }
    }

    public bool HasRightStickButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.RightThumb) != 0;
      }
    }

    public bool HasStartButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Start) != 0;
      }
    }

    public bool HasXButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.X) != 0;
      }
    }

    public bool HasYButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.Y) != 0;
      }
    }

    public bool HasBigButton
    {
      get
      {
        return (ushort)(this._caps.GamePad.Buttons & ButtonValues.BigButton) != 0;
      }
    }

    public bool HasLeftXThumbStick
    {
      get
      {
        return this._caps.GamePad.ThumbLX != 0;
      }
    }

    public bool HasLeftYThumbStick
    {
      get
      {
        return this._caps.GamePad.ThumbLY != 0;
      }
    }

    public bool HasRightXThumbStick
    {
      get
      {
        return this._caps.GamePad.ThumbRX != 0;
      }
    }

    public bool HasRightYThumbStick
    {
      get
      {
        return this._caps.GamePad.ThumbRY != 0;
      }
    }

    public bool HasLeftTrigger
    {
      get
      {
        return this._caps.GamePad.LeftTrigger != 0;
      }
    }

    public bool HasRightTrigger
    {
      get
      {
        return this._caps.GamePad.RightTrigger != 0;
      }
    }

    public bool HasLeftVibrationMotor
    {
      get
      {
        return this._caps.Vibration.LeftMotorSpeed != 0;
      }
    }

    public bool HasRightVibrationMotor
    {
      get
      {
        return this._caps.Vibration.RightMotorSpeed != 0;
      }
    }

    public bool HasVoiceSupport
    {
      get
      {
        return (this._caps.Flags & 4) != 0;
      }
    }

    internal GamePadCapabilities(ref XINPUT_CAPABILITIES caps, ErrorCodes result)
    {
      this._connected = (result != ErrorCodes.NotConnected);
      this._caps = caps;
    }
  }

  public enum GamePadDeadZone
  {
    None,
    IndependentAxes,
    Circular
  }

  internal static class GamePadDeadZoneUtils
  {
    private const int LeftStickDeadZoneSize = 7849;
    private const int RightStickDeadZoneSize = 8689;
    private const int TriggerDeadZoneSize = 30;

    public static float Clamp(float value, float min, float max)
    {
      value = ((value > max) ? max : value);
      value = ((value < min) ? min : value);
      return value;
    }

    internal static Vec2 ApplyLeftStickDeadZone(int x, int y, GamePadDeadZone deadZoneMode)
    {
      return GamePadDeadZoneUtils.ApplyStickDeadZone(x, y, deadZoneMode, 7849);
    }

    internal static Vec2 ApplyRightStickDeadZone(int x, int y, GamePadDeadZone deadZoneMode)
    {
      return GamePadDeadZoneUtils.ApplyStickDeadZone(x, y, deadZoneMode, 8689);
    }

    private static Vec2 ApplyStickDeadZone(int x, int y, GamePadDeadZone deadZoneMode, int deadZoneSize)
    {
      Vec2 result;
      if (deadZoneMode == GamePadDeadZone.IndependentAxes)
      {
        result.X = GamePadDeadZoneUtils.ApplyLinearDeadZone((float)x, 32767f, (float)deadZoneSize);
        result.Y = GamePadDeadZoneUtils.ApplyLinearDeadZone((float)y, 32767f, (float)deadZoneSize);
      }
      else
      {
        if (deadZoneMode == GamePadDeadZone.Circular)
        {
          float num = (float)Math.Sqrt((double)(x * x + y * y));
          float num2 = GamePadDeadZoneUtils.ApplyLinearDeadZone(num, 32767f, (float)deadZoneSize);
          float num3 = (num2 > 0f) ? (num2 / num) : 0f;
          result.X = Clamp((float)x * num3, -1f, 1f);
          result.Y = Clamp((float)y * num3, -1f, 1f);
        }
        else
        {
          result.X = GamePadDeadZoneUtils.ApplyLinearDeadZone((float)x, 32767f, 0f);
          result.Y = GamePadDeadZoneUtils.ApplyLinearDeadZone((float)y, 32767f, 0f);
        }
      }
      return result;
    }

    internal static float ApplyTriggerDeadZone(int value, GamePadDeadZone deadZoneMode)
    {
      if (deadZoneMode == GamePadDeadZone.None)
      {
        return GamePadDeadZoneUtils.ApplyLinearDeadZone((float)value, 255f, 0f);
      }
      return GamePadDeadZoneUtils.ApplyLinearDeadZone((float)value, 255f, 30f);
    }

    private static float ApplyLinearDeadZone(float value, float maxValue, float deadZoneSize)
    {
      if (value < -deadZoneSize)
      {
        value += deadZoneSize;
      }
      else
      {
        if (value <= deadZoneSize)
        {
          return 0f;
        }
        value -= deadZoneSize;
      }
      float value2 = value / (maxValue - deadZoneSize);
      return Clamp(value2, -1f, 1f);
    }
  }

  public struct GamePadDPad
  {
    internal ButtonState _up;
    internal ButtonState _right;
    internal ButtonState _down;
    internal ButtonState _left;

    public ButtonState Up
    {
      get
      {
        return this._up;
      }
    }

    public ButtonState Down
    {
      get
      {
        return this._down;
      }
    }

    public ButtonState Right
    {
      get
      {
        return this._right;
      }
    }

    public ButtonState Left
    {
      get
      {
        return this._left;
      }
    }

    public GamePadDPad(ButtonState upValue, ButtonState downValue, ButtonState leftValue, ButtonState rightValue)
    {
      this._up = upValue;
      this._right = rightValue;
      this._down = downValue;
      this._left = leftValue;
    }

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != base.GetType()) && this == (GamePadDPad)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      string text = string.Empty;
      if (this._up == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Up";
      }
      if (this._down == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Down";
      }
      if (this._left == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Left";
      }
      if (this._right == ButtonState.Pressed)
      {
        text = text + ((text.Length != 0) ? " " : "") + "Right";
      }
      if (text.Length == 0)
      {
        text = "None";
      }
      return string.Format(CultureInfo.CurrentCulture, "{{DPad:{0}}}", new object[]
      {
        text
      });
    }

    public static bool operator ==(GamePadDPad left, GamePadDPad right)
    {
      return left._up == right._up && left._down == right._down && left._left == right._left && left._right == right._right;
    }

    public static bool operator !=(GamePadDPad left, GamePadDPad right)
    {
      return left._up != right._up || left._down != right._down || left._left != right._left || left._right != right._right;
    }
  }

  public struct GamePadState
  {
    private const int _normalButtonMask = 64511;
    private bool _connected;
    private int _packet;
    private GamePadThumbSticks _thumbs;
    private GamePadTriggers _triggers;
    private GamePadButtons _buttons;
    private GamePadDPad _dpad;
    private XINPUT_STATE _state;

    public GamePadButtons Buttons
    {
      get
      {
        return this._buttons;
      }
    }

    public GamePadDPad DPad
    {
      get
      {
        return this._dpad;
      }
    }

    public bool IsConnected
    {
      get
      {
        return this._connected;
      }
    }

    public int PacketNumber
    {
      get
      {
        return this._packet;
      }
    }

    public GamePadThumbSticks ThumbSticks
    {
      get
      {
        return this._thumbs;
      }
    }

    public GamePadTriggers Triggers
    {
      get
      {
        return this._triggers;
      }
    }

    public GamePadState(GamePadThumbSticks thumbSticks, GamePadTriggers triggers, GamePadButtons buttons, GamePadDPad dPad)
    {
      this._packet = 0;
      this._connected = true;
      this._thumbs = thumbSticks;
      this._triggers = triggers;
      this._buttons = buttons;
      this._dpad = dPad;
      this._state = default(XINPUT_STATE);
      this.FillInternalState();
    }

    public GamePadState(Vec2 leftThumbStick, Vec2 rightThumbStick, float leftTrigger, float rightTrigger, params XButtons[] buttons)
    {
      this._packet = 0;
      this._connected = true;
      this._thumbs = new GamePadThumbSticks(leftThumbStick, rightThumbStick);
      this._triggers = new GamePadTriggers(leftTrigger, rightTrigger);
      XButtons buttons2 = (XButtons)0;
      if (buttons != null)
      {
        for (int i = 0; i < buttons.Length; i++)
        {
          buttons2 |= buttons[i];
        }
      }
      this._buttons = new GamePadButtons(buttons2);
      this._dpad = default(GamePadDPad);
      this._dpad._down = (((buttons2 & XButtons.DPadDown) != (XButtons)0) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._up = (((buttons2 & XButtons.DPadUp) != (XButtons)0) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._left = (((buttons2 & XButtons.DPadLeft) != (XButtons)0) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._right = (((buttons2 & XButtons.DPadRight) != (XButtons)0) ? ButtonState.Pressed : ButtonState.Released);
      this._state = default(XINPUT_STATE);
      this.FillInternalState();
    }

    private void FillInternalState()
    {
      this._state.PacketNumber = 0;
      if (this.Buttons.A == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.A);
      }
      if (this.Buttons.B == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.B);
      }
      if (this.Buttons.X == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.X);
      }
      if (this.Buttons.Y == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Y);
      }
      if (this.Buttons.Back == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Back);
      }
      if (this.Buttons.LeftShoulder == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.LeftShoulder);
      }
      if (this.Buttons.LeftStick == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.LeftThumb);
      }
      if (this.Buttons.RightShoulder == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.RightShoulder);
      }
      if (this.Buttons.RightStick == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.RightThumb);
      }
      if (this.Buttons.Start == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Start);
      }
      if (this.Buttons.BigButton == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.BigButton);
      }
      if (this.DPad.Up == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Up);
      }
      if (this.DPad.Down == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Down);
      }
      if (this.DPad.Right == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Right);
      }
      if (this.DPad.Left == ButtonState.Pressed)
      {
        this._state.GamePad.Buttons = (this._state.GamePad.Buttons | ButtonValues.Left);
      }
      this._state.GamePad.LeftTrigger = (byte)(this._triggers._left * 255f);
      this._state.GamePad.RightTrigger = (byte)(this._triggers._right * 255f);
      this._state.GamePad.ThumbLX = (short)(this._thumbs._left.X * 32767f);
      this._state.GamePad.ThumbLY = (short)(this._thumbs._left.Y * 32767f);
      this._state.GamePad.ThumbRX = (short)(this._thumbs._right.X * 32767f);
      this._state.GamePad.ThumbRY = (short)(this._thumbs._right.Y * 32767f);
    }

    internal GamePadState(ref XINPUT_STATE pState, ErrorCodes result, GamePadDeadZone deadZoneMode)
    {
      this._state = pState;
      this._connected = (result != ErrorCodes.NotConnected);
      this._packet = pState.PacketNumber;
      this._buttons._a = (((ushort)(pState.GamePad.Buttons & ButtonValues.A) == 4096) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._b = (((ushort)(pState.GamePad.Buttons & ButtonValues.B) == 8192) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._x = (((ushort)(pState.GamePad.Buttons & ButtonValues.X) == 16384) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._y = (((ushort)(pState.GamePad.Buttons & ButtonValues.Y) == 32768) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._start = (((ushort)(pState.GamePad.Buttons & ButtonValues.Start) == 16) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._back = (((ushort)(pState.GamePad.Buttons & ButtonValues.Back) == 32) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._leftStick = (((ushort)(pState.GamePad.Buttons & ButtonValues.LeftThumb) == 64) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._rightStick = (((ushort)(pState.GamePad.Buttons & ButtonValues.RightThumb) == 128) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._leftShoulder = (((ushort)(pState.GamePad.Buttons & ButtonValues.LeftShoulder) == 256) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._rightShoulder = (((ushort)(pState.GamePad.Buttons & ButtonValues.RightShoulder) == 512) ? ButtonState.Pressed : ButtonState.Released);
      this._buttons._bigButton = (((ushort)(pState.GamePad.Buttons & ButtonValues.BigButton) == 2048) ? ButtonState.Pressed : ButtonState.Released);
      this._triggers._left = GamePadDeadZoneUtils.ApplyTriggerDeadZone((int)pState.GamePad.LeftTrigger, deadZoneMode);
      this._triggers._right = GamePadDeadZoneUtils.ApplyTriggerDeadZone((int)pState.GamePad.RightTrigger, deadZoneMode);
      this._thumbs._left = GamePadDeadZoneUtils.ApplyLeftStickDeadZone((int)pState.GamePad.ThumbLX, (int)pState.GamePad.ThumbLY, deadZoneMode);
      this._thumbs._right = GamePadDeadZoneUtils.ApplyRightStickDeadZone((int)pState.GamePad.ThumbRX, (int)pState.GamePad.ThumbRY, deadZoneMode);
      this._dpad._down = (((ushort)(pState.GamePad.Buttons & ButtonValues.Down) == 2) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._up = (((ushort)(pState.GamePad.Buttons & ButtonValues.Up) == 1) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._left = (((ushort)(pState.GamePad.Buttons & ButtonValues.Left) == 4) ? ButtonState.Pressed : ButtonState.Released);
      this._dpad._right = (((ushort)(pState.GamePad.Buttons & ButtonValues.Right) == 8) ? ButtonState.Pressed : ButtonState.Released);
    }

    public bool IsButtonDown(XButtons button)
    {
      XButtons buttons = (XButtons)(this._state.GamePad.Buttons & (ButtonValues.A | ButtonValues.B | ButtonValues.Back | ButtonValues.Down | ButtonValues.Left | ButtonValues.LeftShoulder | ButtonValues.LeftThumb | ButtonValues.Right | ButtonValues.RightShoulder | ButtonValues.RightThumb | ButtonValues.Start | ButtonValues.Up | ButtonValues.X | ButtonValues.Y | ButtonValues.BigButton));
      if ((button & XButtons.LeftThumbstickLeft) == XButtons.LeftThumbstickLeft && GamePadDeadZoneUtils.ApplyLeftStickDeadZone((int)this._state.GamePad.ThumbLX, (int)this._state.GamePad.ThumbLY, GamePadDeadZone.IndependentAxes).X < 0f)
      {
        buttons |= XButtons.LeftThumbstickLeft;
      }
      if ((button & XButtons.LeftThumbstickRight) == XButtons.LeftThumbstickRight && GamePadDeadZoneUtils.ApplyLeftStickDeadZone((int)this._state.GamePad.ThumbLX, (int)this._state.GamePad.ThumbLY, GamePadDeadZone.IndependentAxes).X > 0f)
      {
        buttons |= XButtons.LeftThumbstickRight;
      }
      if ((button & XButtons.LeftThumbstickDown) == XButtons.LeftThumbstickDown && GamePadDeadZoneUtils.ApplyLeftStickDeadZone((int)this._state.GamePad.ThumbLX, (int)this._state.GamePad.ThumbLY, GamePadDeadZone.IndependentAxes).Y < 0f)
      {
        buttons |= XButtons.LeftThumbstickDown;
      }
      if ((button & XButtons.LeftThumbstickUp) == XButtons.LeftThumbstickUp && GamePadDeadZoneUtils.ApplyLeftStickDeadZone((int)this._state.GamePad.ThumbLX, (int)this._state.GamePad.ThumbLY, GamePadDeadZone.IndependentAxes).Y > 0f)
      {
        buttons |= XButtons.LeftThumbstickUp;
      }
      if ((button & XButtons.RightThumbstickLeft) == XButtons.RightThumbstickLeft && GamePadDeadZoneUtils.ApplyRightStickDeadZone((int)this._state.GamePad.ThumbRX, (int)this._state.GamePad.ThumbRY, GamePadDeadZone.IndependentAxes).X < 0f)
      {
        buttons |= XButtons.RightThumbstickLeft;
      }
      if ((button & XButtons.RightThumbstickRight) == XButtons.RightThumbstickRight && GamePadDeadZoneUtils.ApplyRightStickDeadZone((int)this._state.GamePad.ThumbRX, (int)this._state.GamePad.ThumbRY, GamePadDeadZone.IndependentAxes).X > 0f)
      {
        buttons |= XButtons.RightThumbstickRight;
      }
      if ((button & XButtons.RightThumbstickDown) == XButtons.RightThumbstickDown && GamePadDeadZoneUtils.ApplyRightStickDeadZone((int)this._state.GamePad.ThumbRX, (int)this._state.GamePad.ThumbRY, GamePadDeadZone.IndependentAxes).Y < 0f)
      {
        buttons |= XButtons.RightThumbstickDown;
      }
      if ((button & XButtons.RightThumbstickUp) == XButtons.RightThumbstickUp && GamePadDeadZoneUtils.ApplyRightStickDeadZone((int)this._state.GamePad.ThumbRX, (int)this._state.GamePad.ThumbRY, GamePadDeadZone.IndependentAxes).Y > 0f)
      {
        buttons |= XButtons.RightThumbstickUp;
      }
      if ((button & XButtons.LeftTrigger) == XButtons.LeftTrigger && GamePadDeadZoneUtils.ApplyTriggerDeadZone((int)this._state.GamePad.LeftTrigger, GamePadDeadZone.IndependentAxes) > 0f)
      {
        buttons |= XButtons.LeftTrigger;
      }
      if ((button & XButtons.RightTrigger) == XButtons.RightTrigger && GamePadDeadZoneUtils.ApplyTriggerDeadZone((int)this._state.GamePad.RightTrigger, GamePadDeadZone.IndependentAxes) > 0f)
      {
        buttons |= XButtons.RightTrigger;
      }
      return (button & buttons) == button;
    }

    public bool IsButtonUp(XButtons button)
    {
      return !this.IsButtonDown(button);
    }

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != base.GetType()) && this == (GamePadState)obj;
    }

    public override int GetHashCode()
    {
      return this._thumbs.GetHashCode() ^ this._triggers.GetHashCode() ^ (this._buttons.GetHashCode() ^ this._connected.GetHashCode()) ^ (this._dpad.GetHashCode() ^ this._packet.GetHashCode());
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.CurrentCulture, "{{IsConnected:{0}}}", new object[]
      {
        this._connected
      });
    }

    public static bool operator ==(GamePadState left, GamePadState right)
    {
      return left._connected == right._connected && left._packet == right._packet && left._thumbs == right._thumbs && left._triggers == right._triggers && left._buttons == right._buttons && left._dpad == right._dpad;
    }

    public static bool operator !=(GamePadState left, GamePadState right)
    {
      return left._connected != right._connected || left._packet != right._packet || left._thumbs != right._thumbs || left._triggers != right._triggers || left._buttons != right._buttons || left._dpad != right._dpad;
    }
  }

  public struct GamePadThumbSticks
  {
    internal Vec2 _left;
    internal Vec2 _right;

    public Vec2 Left
    {
      get
      {
        return this._left;
      }
    }

    public Vec2 Right
    {
      get
      {
        return this._right;
      }
    }

    public GamePadThumbSticks(Vec2 leftThumbstick, Vec2 rightThumbstick)
    {
      this._left = leftThumbstick;
      this._right = rightThumbstick;
      this._left = Vec2.Min(this._left, Vec2.One);
      this._left = Vec2.Max(this._left, -Vec2.One);
      this._right = Vec2.Min(this._right, Vec2.One);
      this._right = Vec2.Max(this._right, -Vec2.One);
    }

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != base.GetType()) && this == (GamePadThumbSticks)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.CurrentCulture, "{{Left:{0} Right:{1}}}", new object[]
      {
        this._left,
        this._right
      });
    }

    public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
    {
      return left._left == right._left && left._right == right._right;
    }

    public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
    {
      return left._left != right._left || left._right != right._right;
    }
  }

  public struct GamePadTriggers
  {
    internal float _left;
    internal float _right;

    public float Left
    {
      get
      {
        return this._left;
      }
    }

    public float Right
    {
      get
      {
        return this._right;
      }
    }

    public GamePadTriggers(float leftTrigger, float rightTrigger)
    {
      this._left = leftTrigger;
      this._right = rightTrigger;
      this._left = Math.Min(this._left, 1f);
      this._left = Math.Max(this._left, 0f);
      this._right = Math.Min(this._right, 1f);
      this._right = Math.Max(this._right, 0f);
    }

    public override bool Equals(object obj)
    {
      return obj != null && !(obj.GetType() != base.GetType()) && this == (GamePadTriggers)obj;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return string.Format(CultureInfo.CurrentCulture, "{{Left:{0} Right:{1}}}", new object[]
      {
        this._left,
        this._right
      });
    }

    public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
    {
      return left._left == right._left && left._right == right._right;
    }

    public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
    {
      return left._left != right._left || left._right != right._right;
    }
  }

  public enum GamePadType
  {
    Unknown,
    ArcadeStick = 3,
    DancePad = 5,
    FlightStick = 4,
    GamePad = 1,
    Wheel,
    Guitar = 6,
    DrumKit = 8,
    AlternateGuitar = 7,
    BigButtonPad = 768
  }

  public enum PlayerIndex
  {
    One,
    Two,
    Three,
    Four
  }

  internal static class UnsafeNativeMethods
  {
    [DllImport("xinput1_3.dll", EntryPoint = "XInputGetState")]
    public static extern ErrorCodes GetState(PlayerIndex playerIndex, out XINPUT_STATE pState);
    [DllImport("xinput1_3.dll", EntryPoint = "XInputSetState")]
    public static extern ErrorCodes SetState(PlayerIndex playerIndex, ref XINPUT_VIBRATION pVibration);
    [DllImport("xinput1_3.dll", EntryPoint = "XInputGetCapabilities")]
    public static extern ErrorCodes GetCaps(PlayerIndex playerIndex, uint flags, out XINPUT_CAPABILITIES pCaps);
    [DllImport("User32.dll", EntryPoint = "GetWindowLong")]
    public static extern IntPtr GetWindowLong32(IntPtr hWnd, int nIndex);
    [DllImport("User32.dll", EntryPoint = "GetWindowLongPtr")]
    public static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
    [DllImport("User32.dll", EntryPoint = "SetWindowLong")]
    public static extern IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr newValue);
    [DllImport("User32.dll", EntryPoint = "SetWindowLongPtr")]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr newValue);
    [DllImport("User32.dll")]
    public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("User32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    [DllImport("User32.dll")]
    public static extern uint RegisterWindowMessage(string lpString);
  }

  public struct Vec2 : IEquatable<Vec2>
  {
    public float X;
    public float Y;
    private static Vec2 _zero = default(Vec2);
    private static Vec2 _one = new Vec2(1f, 1f);
    private static Vec2 _unitX = new Vec2(1f, 0f);
    private static Vec2 _unitY = new Vec2(0f, 1f);

    public static Vec2 Zero
    {
      get
      {
        return Vec2._zero;
      }
    }

    public static Vec2 One
    {
      get
      {
        return Vec2._one;
      }
    }

    public static Vec2 UnitX
    {
      get
      {
        return Vec2._unitX;
      }
    }

    public static Vec2 UnitY
    {
      get
      {
        return Vec2._unitY;
      }
    }

    public Vec2(float x, float y)
    {
      this.X = x;
      this.Y = y;
    }

    public Vec2(float value)
    {
      this.Y = value;
      this.X = value;
    }

    public override string ToString()
    {
      CultureInfo currentCulture = CultureInfo.CurrentCulture;
      return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[]
      {
        this.X.ToString(currentCulture),
        this.Y.ToString(currentCulture)
      });
    }

    public bool Equals(Vec2 other)
    {
      return this.X == other.X && this.Y == other.Y;
    }

    public override bool Equals(object obj)
    {
      bool result = false;
      if (obj is Vec2)
      {
        result = this.Equals((Vec2)obj);
      }
      return result;
    }

    public override int GetHashCode()
    {
      return this.X.GetHashCode() + this.Y.GetHashCode();
    }

    public float Length()
    {
      float num = this.X * this.X + this.Y * this.Y;
      return (float)Math.Sqrt((double)num);
    }

    public float LengthSquared()
    {
      return this.X * this.X + this.Y * this.Y;
    }

    public static float Distance(Vec2 value1, Vec2 value2)
    {
      float num = value1.X - value2.X;
      float num2 = value1.Y - value2.Y;
      float num3 = num * num + num2 * num2;
      return (float)Math.Sqrt((double)num3);
    }

    public static void Distance(ref Vec2 value1, ref Vec2 value2, out float result)
    {
      float num = value1.X - value2.X;
      float num2 = value1.Y - value2.Y;
      float num3 = num * num + num2 * num2;
      result = (float)Math.Sqrt((double)num3);
    }

    public static float DistanceSquared(Vec2 value1, Vec2 value2)
    {
      float num = value1.X - value2.X;
      float num2 = value1.Y - value2.Y;
      return num * num + num2 * num2;
    }

    public static void DistanceSquared(ref Vec2 value1, ref Vec2 value2, out float result)
    {
      float num = value1.X - value2.X;
      float num2 = value1.Y - value2.Y;
      result = num * num + num2 * num2;
    }

    public static float Dot(Vec2 value1, Vec2 value2)
    {
      return value1.X * value2.X + value1.Y * value2.Y;
    }

    public static void Dot(ref Vec2 value1, ref Vec2 value2, out float result)
    {
      result = value1.X * value2.X + value1.Y * value2.Y;
    }

    public void Normalize()
    {
      float num = this.X * this.X + this.Y * this.Y;
      float num2 = 1f / (float)Math.Sqrt((double)num);
      this.X *= num2;
      this.Y *= num2;
    }

    public static Vec2 Normalize(Vec2 value)
    {
      float num = value.X * value.X + value.Y * value.Y;
      float num2 = 1f / (float)Math.Sqrt((double)num);
      Vec2 result;
      result.X = value.X * num2;
      result.Y = value.Y * num2;
      return result;
    }

    public static void Normalize(ref Vec2 value, out Vec2 result)
    {
      float num = value.X * value.X + value.Y * value.Y;
      float num2 = 1f / (float)Math.Sqrt((double)num);
      result.X = value.X * num2;
      result.Y = value.Y * num2;
    }

    public static Vec2 Reflect(Vec2 vector, Vec2 normal)
    {
      float num = vector.X * normal.X + vector.Y * normal.Y;
      Vec2 result;
      result.X = vector.X - 2f * num * normal.X;
      result.Y = vector.Y - 2f * num * normal.Y;
      return result;
    }

    public static void Reflect(ref Vec2 vector, ref Vec2 normal, out Vec2 result)
    {
      float num = vector.X * normal.X + vector.Y * normal.Y;
      result.X = vector.X - 2f * num * normal.X;
      result.Y = vector.Y - 2f * num * normal.Y;
    }

    public static Vec2 Min(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = ((value1.X < value2.X) ? value1.X : value2.X);
      result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
      return result;
    }

    public static void Min(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = ((value1.X < value2.X) ? value1.X : value2.X);
      result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
    }

    public static Vec2 Max(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = ((value1.X > value2.X) ? value1.X : value2.X);
      result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
      return result;
    }

    public static void Max(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = ((value1.X > value2.X) ? value1.X : value2.X);
      result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
    }

    public static Vec2 Clamp(Vec2 value1, Vec2 min, Vec2 max)
    {
      float num = value1.X;
      num = ((num > max.X) ? max.X : num);
      num = ((num < min.X) ? min.X : num);
      float num2 = value1.Y;
      num2 = ((num2 > max.Y) ? max.Y : num2);
      num2 = ((num2 < min.Y) ? min.Y : num2);
      Vec2 result;
      result.X = num;
      result.Y = num2;
      return result;
    }

    public static void Clamp(ref Vec2 value1, ref Vec2 min, ref Vec2 max, out Vec2 result)
    {
      float num = value1.X;
      num = ((num > max.X) ? max.X : num);
      num = ((num < min.X) ? min.X : num);
      float num2 = value1.Y;
      num2 = ((num2 > max.Y) ? max.Y : num2);
      num2 = ((num2 < min.Y) ? min.Y : num2);
      result.X = num;
      result.Y = num2;
    }

    public static Vec2 Lerp(Vec2 value1, Vec2 value2, float amount)
    {
      Vec2 result;
      result.X = value1.X + (value2.X - value1.X) * amount;
      result.Y = value1.Y + (value2.Y - value1.Y) * amount;
      return result;
    }

    public static void Lerp(ref Vec2 value1, ref Vec2 value2, float amount, out Vec2 result)
    {
      result.X = value1.X + (value2.X - value1.X) * amount;
      result.Y = value1.Y + (value2.Y - value1.Y) * amount;
    }

    public static Vec2 Barycentric(Vec2 value1, Vec2 value2, Vec2 value3, float amount1, float amount2)
    {
      Vec2 result;
      result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
      result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
      return result;
    }

    public static void Barycentric(ref Vec2 value1, ref Vec2 value2, ref Vec2 value3, float amount1, float amount2, out Vec2 result)
    {
      result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
      result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
    }

    public static Vec2 SmoothStep(Vec2 value1, Vec2 value2, float amount)
    {
      amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
      amount = amount * amount * (3f - 2f * amount);
      Vec2 result;
      result.X = value1.X + (value2.X - value1.X) * amount;
      result.Y = value1.Y + (value2.Y - value1.Y) * amount;
      return result;
    }

    public static void SmoothStep(ref Vec2 value1, ref Vec2 value2, float amount, out Vec2 result)
    {
      amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
      amount = amount * amount * (3f - 2f * amount);
      result.X = value1.X + (value2.X - value1.X) * amount;
      result.Y = value1.Y + (value2.Y - value1.Y) * amount;
    }

    public static Vec2 CatmullRom(Vec2 value1, Vec2 value2, Vec2 value3, Vec2 value4, float amount)
    {
      float num = amount * amount;
      float num2 = amount * num;
      Vec2 result;
      result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
      result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
      return result;
    }

    public static void CatmullRom(ref Vec2 value1, ref Vec2 value2, ref Vec2 value3, ref Vec2 value4, float amount, out Vec2 result)
    {
      float num = amount * amount;
      float num2 = amount * num;
      result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
      result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
    }

    public static Vec2 Hermite(Vec2 value1, Vec2 tangent1, Vec2 value2, Vec2 tangent2, float amount)
    {
      float num = amount * amount;
      float num2 = amount * num;
      float num3 = 2f * num2 - 3f * num + 1f;
      float num4 = -2f * num2 + 3f * num;
      float num5 = num2 - 2f * num + amount;
      float num6 = num2 - num;
      Vec2 result;
      result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
      result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
      return result;
    }

    public static void Hermite(ref Vec2 value1, ref Vec2 tangent1, ref Vec2 value2, ref Vec2 tangent2, float amount, out Vec2 result)
    {
      float num = amount * amount;
      float num2 = amount * num;
      float num3 = 2f * num2 - 3f * num + 1f;
      float num4 = -2f * num2 + 3f * num;
      float num5 = num2 - 2f * num + amount;
      float num6 = num2 - num;
      result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
      result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
    }

    public static Vec2 Negate(Vec2 value)
    {
      Vec2 result;
      result.X = -value.X;
      result.Y = -value.Y;
      return result;
    }

    public static void Negate(ref Vec2 value, out Vec2 result)
    {
      result.X = -value.X;
      result.Y = -value.Y;
    }

    public static Vec2 Add(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X + value2.X;
      result.Y = value1.Y + value2.Y;
      return result;
    }

    public static void Add(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = value1.X + value2.X;
      result.Y = value1.Y + value2.Y;
    }

    public static Vec2 Subtract(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X - value2.X;
      result.Y = value1.Y - value2.Y;
      return result;
    }

    public static void Subtract(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = value1.X - value2.X;
      result.Y = value1.Y - value2.Y;
    }

    public static Vec2 Multiply(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X * value2.X;
      result.Y = value1.Y * value2.Y;
      return result;
    }

    public static void Multiply(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = value1.X * value2.X;
      result.Y = value1.Y * value2.Y;
    }

    public static Vec2 Multiply(Vec2 value1, float scaleFactor)
    {
      Vec2 result;
      result.X = value1.X * scaleFactor;
      result.Y = value1.Y * scaleFactor;
      return result;
    }

    public static void Multiply(ref Vec2 value1, float scaleFactor, out Vec2 result)
    {
      result.X = value1.X * scaleFactor;
      result.Y = value1.Y * scaleFactor;
    }

    public static Vec2 Divide(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X / value2.X;
      result.Y = value1.Y / value2.Y;
      return result;
    }

    public static void Divide(ref Vec2 value1, ref Vec2 value2, out Vec2 result)
    {
      result.X = value1.X / value2.X;
      result.Y = value1.Y / value2.Y;
    }

    public static Vec2 Divide(Vec2 value1, float divider)
    {
      float num = 1f / divider;
      Vec2 result;
      result.X = value1.X * num;
      result.Y = value1.Y * num;
      return result;
    }

    public static void Divide(ref Vec2 value1, float divider, out Vec2 result)
    {
      float num = 1f / divider;
      result.X = value1.X * num;
      result.Y = value1.Y * num;
    }

    public static Vec2 operator -(Vec2 value)
    {
      Vec2 result;
      result.X = -value.X;
      result.Y = -value.Y;
      return result;
    }

    public static bool operator ==(Vec2 value1, Vec2 value2)
    {
      return value1.X == value2.X && value1.Y == value2.Y;
    }

    public static bool operator !=(Vec2 value1, Vec2 value2)
    {
      return value1.X != value2.X || value1.Y != value2.Y;
    }

    public static Vec2 operator +(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X + value2.X;
      result.Y = value1.Y + value2.Y;
      return result;
    }

    public static Vec2 operator -(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X - value2.X;
      result.Y = value1.Y - value2.Y;
      return result;
    }

    public static Vec2 operator *(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X * value2.X;
      result.Y = value1.Y * value2.Y;
      return result;
    }

    public static Vec2 operator *(Vec2 value, float scaleFactor)
    {
      Vec2 result;
      result.X = value.X * scaleFactor;
      result.Y = value.Y * scaleFactor;
      return result;
    }

    public static Vec2 operator *(float scaleFactor, Vec2 value)
    {
      Vec2 result;
      result.X = value.X * scaleFactor;
      result.Y = value.Y * scaleFactor;
      return result;
    }

    public static Vec2 operator /(Vec2 value1, Vec2 value2)
    {
      Vec2 result;
      result.X = value1.X / value2.X;
      result.Y = value1.Y / value2.Y;
      return result;
    }

    public static Vec2 operator /(Vec2 value1, float divider)
    {
      float num = 1f / divider;
      Vec2 result;
      result.X = value1.X * num;
      result.Y = value1.Y * num;
      return result;
    }
  }

  [Flags]
  public enum XButtons : uint
  {
    A = 4096,
    B = 8192,
    X = 16384,
    Y = 32768,
    Back = 32,
    Start = 16,
    DPadUp = 1,
    DPadDown = 2,
    DPadLeft = 4,
    DPadRight = 8,
    LeftShoulder = 256,
    RightShoulder = 512,
    LeftStick = 64,
    RightStick = 128,
    BigButton = 2048,
    LeftThumbstickLeft = 2097152,
    LeftThumbstickRight = 1073741824,
    LeftThumbstickDown = 536870912,
    LeftThumbstickUp = 268435456,
    RightThumbstickLeft = 134217728,
    RightThumbstickRight = 67108864,
    RightThumbstickDown = 33554432,
    RightThumbstickUp = 16777216,
    LeftTrigger = 8388608,
    RightTrigger = 4194304,
    Guide = 2147483648,
  }

  internal struct XINPUT_CAPABILITIES
  {
    public byte Type;
    public byte SubType;
    public ushort Flags;
    public XINPUT_GAMEPAD GamePad;
    public XINPUT_VIBRATION Vibration;
  }

  internal struct XINPUT_GAMEPAD
  {
    public ButtonValues Buttons;
    public byte LeftTrigger;
    public byte RightTrigger;
    public short ThumbLX;
    public short ThumbLY;
    public short ThumbRX;
    public short ThumbRY;
  }

  internal struct XINPUT_STATE
  {
    public int PacketNumber;
    public XINPUT_GAMEPAD GamePad;
  }

  internal struct XINPUT_VIBRATION
  {
    public short LeftMotorSpeed;
    public short RightMotorSpeed;
  }
}