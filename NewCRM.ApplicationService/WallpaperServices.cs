﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NewCRM.Application.Services.Interface;
using NewCRM.Domain.Entitys.System;
using NewCRM.Domain.Services;
using NewCRM.Domain.Services.Interface;
using NewCRM.Domain.ValueObject;
using NewCRM.Dto;
using NewCRM.Infrastructure.CommonTools;
using NewLib;
using static NewCRM.Infrastructure.CommonTools.CacheKey;

namespace NewCRM.Application.Services
{
    public class WallpaperServices : BaseServiceContext, IWallpaperServices
    {
        private readonly IWallpaperContext _wallpaperContext;
        public WallpaperServices(IWallpaperContext wallpaperContext) => _wallpaperContext = wallpaperContext;

        public async Task<List<WallpaperDto>> GetWallpapersAsync()
        {
            var result = await _wallpaperContext.GetWallpapersAsync();
            return result.Select(s => new WallpaperDto
            {
                AccountId = s.AccountId,
                Height = s.Height,
                Id = s.Id,
                Md5 = s.Md5,
                ShortUrl = s.ShortUrl,
                Source = (Int32)s.Source,
                Title = s.Title,
                Url = s.Url,
                Width = s.Width
            }).ToList();
        }

        public async Task<Tuple<Int32, String>> AddWallpaperAsync(WallpaperDto wallpaperDto)
        {
            Parameter.Validate(wallpaperDto);
            return await _wallpaperContext.AddWallpaperAsync(wallpaperDto.ConvertToModel<WallpaperDto, Wallpaper>());
        }

        public async Task<List<WallpaperDto>> GetUploadWallpaperAsync(Int32 accountId)
        {
            Parameter.Validate(accountId);
            return (await _wallpaperContext.GetUploadWallpaperAsync(accountId)).Select(s => new WallpaperDto
            {
                AccountId = s.AccountId,
                Height = s.Height,
                Id = s.Id,
                Md5 = s.Md5,
                ShortUrl = s.ShortUrl,
                Source = (Int32)s.Source,
                Title = s.Title,
                Url = ProfileManager.FileUrl + s.Url,
                Width = s.Width
            }).ToList();
        }

        public async Task<Tuple<Int32, String>> AddWebWallpaperAsync(Int32 accountId, String url)
        {
            Parameter.Validate(accountId).Validate(url);

            var imageTitle = Path.GetFileNameWithoutExtension(url);
            Image image;

            using (var stream = await new HttpClient().GetStreamAsync(new Uri(url)))
            using (image = Image.FromStream(stream))
            {
                var wallpaperMd5 = FileHelper.GetMD5(stream);
                var webWallpaper = await GetUploadWallpaperAsync(wallpaperMd5);
                if (webWallpaper != null)
                {
                    return new Tuple<Int32, String>(webWallpaper.Id, webWallpaper.ShortUrl);
                }

                var wallpaperResult = await AddWallpaperAsync(new WallpaperDto
                {
                    Width = image.Width,
                    Height = image.Height,
                    Source = (Int32)WallpaperSource.Web,
                    Title = imageTitle,
                    Url = url,
                    AccountId = accountId,
                    Md5 = wallpaperMd5,
                    ShortUrl = url
                });
                return new Tuple<Int32, String>(wallpaperResult.Item1, wallpaperResult.Item2);
            }
        }

        public async Task<WallpaperDto> GetUploadWallpaperAsync(String md5)
        {
            Parameter.Validate(md5);
            var result = await _wallpaperContext.GetUploadWallpaperAsync(md5);
            return new WallpaperDto
            {
                AccountId = result.AccountId,
                Height = result.Height,
                Width = result.Width,
                Id = result.Id,
                Md5 = result.Md5,
                ShortUrl = result.ShortUrl,
                Source = (Int32)result.Source,
                Title = result.Title,
                Url = result.Url
            };
        }

        public async Task ModifyWallpaperModeAsync(Int32 accountId, String newMode)
        {
            Parameter.Validate(accountId).Validate(newMode);
            await _wallpaperContext.ModifyWallpaperModeAsync(accountId, newMode);
            RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
        }

        public async Task ModifyWallpaperAsync(Int32 accountId, Int32 newWallpaperId)
        {
            Parameter.Validate(accountId).Validate(newWallpaperId);
            await _wallpaperContext.ModifyWallpaperAsync(accountId, newWallpaperId);
            RemoveOldKeyWhenModify(new ConfigCacheKey(accountId));
        }

        public async Task RemoveWallpaperAsync(Int32 accountId, Int32 wallpaperId)
        {
            Parameter.Validate(accountId).Validate(wallpaperId);
            await _wallpaperContext.RemoveWallpaperAsync(accountId, wallpaperId);
        }
    }
}
