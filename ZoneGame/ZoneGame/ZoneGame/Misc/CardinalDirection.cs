#region File Description
//-----------------------------------------------------------------------------
// Direction.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace ZoneGame
{
    /// <summary>
    /// Cardinal and ordinal directions, primary for sprite orientation.
    /// </summary>
    public enum CardinalDirection
    {
        North,
        NorthNortheast,
        NorthEast,
        EastNortheast,
        East,
        EastSoutheast,
        SouthEast,
        SouthSoutheast,
        South,
        SouthSouthwest,
        SouthWest,
        WestSouthwest,
        West,
        WestNorthwest,
        NorthWest,
        NorthNorthwest,
    }
}
