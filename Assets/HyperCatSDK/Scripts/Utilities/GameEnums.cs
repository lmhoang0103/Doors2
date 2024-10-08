public enum SceneIndex
{
    None = -1,
    Splash = 0,
    Gameplay
}

public enum PanelType
{
    None,
    Screen,
    Popup,
    Notification,
    Loading
}

public enum TypeSound
{
    None = -1,
    Button = 0,
    OpenPopup = 1,
    ClosePopup = 2,
}

public enum Pool
{
    None
}

public enum RewardType
{
    Coin,
    Gift
}

public enum EasingType
{
    Linear = 1,
    InSine,
    OutSine,
    InOutSine,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    InQuart,
    OutQuart,
    InOutQuart,
    InQuint,
    OutQuint,
    InOutQuint,
    InExpo,
    OutExpo,
    InOutExpo,
    InCirc,
    OutCirc,
    InOutCirc,
    InElastic,
    OutElastic,
    InOutElastic,
    InBack,
    OutBack,
    InOutBack,
    InBounce,
    OutBounce,
    InOutBounce,
    Flash,
    InFlash,
    OutFlash,
    InOutFlash,
    Custom
}

public enum ItemType
{
    Coin,
    GoldenKey,
    NormalKey,
    FlashLight,
    Book,
    Painting,
    Hammer,
    FakeKey,
    Food
}

public enum PaintingFrameType
{
    Square,
    VerticalRectangle,
    HorizontalRectangle,
    Circle,
    Triangle,
    NoShape
}

public enum BookCoverType
{
    Circle,
    Cross,
    Square,
    Triangle,
    NoSymbol
}

public enum RoomDirection
{
    Straight = 0,
    Right = 1,
    Back = 2,
    Left = 3,
}