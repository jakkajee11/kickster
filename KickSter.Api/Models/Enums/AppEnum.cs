using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KickSter.Models.Enums
{
    public enum Gender
    {
        Male    = 0,
        Femal   = 1
    }

    public enum SocialMedia
    {
        FaceBook    = 0,
        Twitter     = 1,
        Line        = 2,
        WhatApps    = 3,
        WeChat      = 4,
        Other       = 99
    }

    public enum PostType
    {
        FindPlayer      = 0,
        FindTeam        = 1,
        Event           = 2,
        Other           = 99,
    }

    public enum PostStatus
    {
        Open        = 0,
        Closed      = 1
    }
}