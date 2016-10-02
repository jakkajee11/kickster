using KickSter.Api.Models;
using KickSter.Api.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace KickSter.Api.Helpers
{

    public class MatchingHelpers
    {
        private static IUserService _userService;
        private static IPostService _postService;
        private static IMapper _mapper;

        public static void Init(Container container)
        {
            // Simulate WebApi Request
            using (container.BeginExecutionContextScope())
            {
                _userService = container.GetInstance<IUserService>();
                _postService = container.GetInstance<IPostService>();
                _mapper = container.GetInstance<IMapper>();
            }            
        }        

        public async static Task<IEnumerable<MatchedPost>> MatchedByUserPreference(string userId)
        {
            var posts = new List<MatchedPost>();
            // Get user's preferences
            var user = await _userService.GetUserById(userId);

            if (user.Preferences == null) return posts;
            // Find posts that matched to user's preferences             
            var days = await _postService.FindPreferenceDaysAsync(user.Preferences.DayTimes);
            var arenas = await _postService.FindArenasAsync(user.Preferences.Arenas);
            var zones = await _postService.FindZonesAsync(user.Preferences.Zones);

            // Map Post to MatchedPost and add to result (posts) with score  
            posts.AddRange(_mapper.Map<IEnumerable<MatchedPost>>(days));
            // 50% matched by day & time
            foreach (var p in posts)
            {
                p.Score = 50;
                p.Day = true;
            }

            foreach (var item in _mapper.Map<IEnumerable<MatchedPost>>(arenas))
            {
                var idx = posts.FindIndex(p => p.Id == item.Id);
                item.Arena = true;
                item.Score = 40;
                if (idx >= 0)
                {
                    posts[idx].Score += 40;
                    posts[idx].Arena = true;
                }
                else
                    posts.Add(item);
            }

            foreach (var item in _mapper.Map<IEnumerable<MatchedPost>>(zones))
            {
                var idx = posts.FindIndex(p => p.Id == item.Id);
                item.Zone = true;
                item.Score = 10;
                if (idx >= 0)
                {
                    posts[idx].Score += 10;
                    posts[idx].Zone = true;
                }
                else
                    posts.Add(item);
            }

            return posts.Where(p=> !p.PostedBy.Id.Equals(userId)).OrderByDescending(p => p.Score);
        }

        public async static Task<IEnumerable<MatchedPost>> MatchedPost(string postId)
        {
            var posts = new List<MatchedPost>();
            // Get post
            var post = await _postService.FindOneAsync(postId);

            // Find posts that matched to user's post
            var days = await _postService.FindPreferenceDaysAsync(post.DayTimes);
            var arenas = await _postService.FindArenasAsync(post.Arenas);
            var zones = await _postService.FindZonesAsync(post.Zones);
            var tags = await _postService.FindTagsAsync(post.Tags);

            // Map Post to MatchedPost and add to result (posts) with score  
            posts.AddRange(_mapper.Map<IEnumerable<MatchedPost>>(days.Where(p=>p.Id != post.Id)));

            foreach (var item in _mapper.Map<IEnumerable<MatchedPost>>(arenas.Where(p => p.Id != post.Id)))
            {
                var idx = posts.FindIndex(p => p.Id == item.Id);
                if (idx >= 0)
                    posts[idx].Score += 1;
                else
                    posts.Add(item);
            }

            foreach (var item in _mapper.Map<IEnumerable<MatchedPost>>(zones.Where(p => p.Id != post.Id)))
            {
                var idx = posts.FindIndex(p => p.Id == item.Id);
                if (idx >= 0)
                    posts[idx].Score += 1;
                else
                    posts.Add(item);
            }

            foreach (var item in _mapper.Map<IEnumerable<MatchedPost>>(tags.Where(p => p.Id != post.Id)))
            {
                var idx = posts.FindIndex(p => p.Id == item.Id);
                if (idx >= 0)
                    posts[idx].Score += 1;
                else
                    posts.Add(item);
            }

            foreach (var item in posts)
            {
                if (item.Type == post.Type)
                    item.Score *= 10;
            }

            return posts.OrderByDescending(p => p.Score);
        }

        public async static Task<IEnumerable<UserInfo>> MatchedFriend(string userId)
        {
            //var friends = new List<UserInfo>();
            var friends = new Dictionary<string, UserInfo>();
            // Get post
            var user = await _userService.GetUserById(userId);
            if (user == null || user.Preferences == null) return friends.Values;
            // Find other users which their preference match user preference
            var users = await _userService.ListAsync();
            foreach(var u in users.Where(up => up.Id != user.Id && up.Preferences != null))
            {
                if (u.Preferences.DayTimes.Count() > 0 && user.Preferences.DayTimes.Count() > 0)
                {
                    if (u.Preferences.DayTimes.Exists(ud => user.Preferences.DayTimes.Select(dt => dt.Day).Contains(ud.Day)))
                    {
                        friends.Add(u.Id, _mapper.Map<UserInfo>(u));

                        continue;
                    }
                        
                }

                if (u.Preferences.Zones.Count() > 0 && user.Preferences.Zones.Count() > 0)
                {
                    if (u.Preferences.Zones.Exists(uz => user.Preferences.Zones.Select(zn => zn.Id).Contains(uz.Id)))
                    {
                        if (!friends.ContainsKey(u.Id))
                        {
                            friends.Add(u.Id, _mapper.Map<UserInfo>(u));

                            continue;
                        }
                            
                    }                                            
                }

                if (u.Preferences.Arenas.Count() > 0 && user.Preferences.Arenas.Count() > 0)
                {
                    if (u.Preferences.Arenas.Exists(ua => user.Preferences.Arenas.Select(an => an.Id).Contains(ua.Id)))
                    {
                        if (!friends.ContainsKey(u.Id))
                        {
                            friends.Add(u.Id, _mapper.Map<UserInfo>(u));

                            continue;
                        }

                    }
                }
            }

            return friends.Values;
        }

    }
}