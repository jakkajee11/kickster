using KickSter.Api.Models;
using System;
using System.Configuration;
using WebApiHttpClient;
using Xunit;
using Shouldly;
using System.Collections.Generic;
using GLib.Common;

namespace KickSter.Api.Test
{
    //[TestClass]
    public class KickSterApiTests
    {
        private readonly HttpClientHelpers _httpClient;
        public KickSterApiTests()
        {
            _httpClient = new HttpClientHelpers(ConfigurationManager.AppSettings["BaseUrl"].ToString());
        }

        [Fact(DisplayName ="Zone_List_Should_Return_All_Zones")]
        public void ZoneListShouldReturnAllZones()
        {
            var result = _httpClient.Get<List<Zone>>("zones/list", null);
            result.Count.ShouldBe(50);
        }

        [Fact(DisplayName = "Zone_List_Page_1_PageSize_5_Should_Return_5_Zones")]
        public void ZoneListPage1PageSize5ShouldReturn5Zones()
        {
            var result = _httpClient.Get<List<Zone>>("zones/list", new Paging { Page = 1, Size = 5 });
            result.Count.ShouldBe(5);
        }

        [Fact(DisplayName = "Arena_List_Should_Return_All_Arenas")]
        public void ArenaListShouldReturnAllArenas()
        {
            var result = _httpClient.Get<List<Arena>>("arenas/list", null);
            result.Count.ShouldBeGreaterThan(0);
        }

        [Fact(DisplayName = "Post_List_Should_Return_All_Posts")]
        public void PostListShouldReturnAllPosts()
        {
            var result = _httpClient.Get<List<Post>>("post/list", null);
            result.Count.ShouldBeGreaterThan(0);
        }
    }
}
