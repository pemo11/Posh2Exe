public class PoshHostRawUserInterface : PSHostRawUserInterface
{
    public override ConsoleColor BackgroundColor
    {
        get { return Console.BackgroundColor; }
        set { Console.BackgroundColor = value; }
    }

    public override ConsoleColor ForegroundColor
    {
        get { return Console.ForegroundColor; }
        set { Console.ForegroundColor = value; }
    }

    public override Size BufferSize
    {
        get { return new Size(Console.BufferWidth, Console.BufferHeight); }
        set { Console.SetBufferSize(value.Width, value.Height); }
    }

    public override Coordinates CursorPosition
    {
        get { throw new NotImplementedException("Not implemented because not needed."); }
        set { throw new NotImplementedException("Not implemented because not needed"); }
    }

    public override int CursorSize
    {
        get { return Console.CursorSize; }
        set { Console.CursorSize = value; }
    }


    public override bool KeyAvailable
    {
        get { return Console.KeyAvailable; }
    }

    public override Size MaxPhysicalWindowSize
    {
        get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
    }

    public override Size MaxWindowSize
    {
        get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
    }

    public override Coordinates WindowPosition
    {
        get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
        set { Console.SetWindowPosition(value.X, value.Y); }
    }

    public override Size WindowSize
    {
        get { return new Size(Console.WindowWidth, Console.WindowHeight); }
        set { Console.SetWindowSize(value.Width, value.Height); }
    }

    public override string WindowTitle
    {
        get { return Console.Title; }
        set { Console.Title = value; }
    }

    public override void FlushInputBuffer()
    {
        // Hier passiert nichts
    }

    public override BufferCell[,] GetBufferContents(Rectangle rectangle)
    {
        throw new NotImplementedException("Not implemented because not needed");
    }

    public override KeyInfo ReadKey(ReadKeyOptions options)
    {
        throw new NotImplementedException("Not implemented because not needed");
    }

    public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
    {
        throw new NotImplementedException("Not implemented because not needed");
    }

    public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
    {
        throw new NotImplementedException("Not implemented because not needed");
    }

    public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
    {
        throw new NotImplementedException("Not implemented because not needed");
    }
}


