﻿// <copyright file="PhysicalFileImageService.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using ImageSharp.Memory;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Returns images stored in the local physical file system.
    /// </summary>
    public class PhysicalFileImageService : IImageService
    {
        /// <inheritdoc/>
        public string Key { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public async Task<bool> IsValidRequestAsync(HttpContext context, IHostingEnvironment environment, ILogger logger, string path)
        {
            // TODO: Either Write proper validation based on static FormatHelper (not written) in base library
            // Or can we get this from the request header (preferred here)?
            return await Task.FromResult(true);
        }

        /// <inheritdoc/>
        public async Task<byte[]> ResolveImageAsync(HttpContext context, IHostingEnvironment environment, ILogger logger, string path)
        {
            // Path has already been correctly parsed before here.
            IFileProvider fileProvider = environment.WebRootFileProvider;
            IFileInfo fileInfo = fileProvider.GetFileInfo(path);
            byte[] buffer;

            // Check to see if the file exists.
            if (!fileInfo.Exists)
            {
                return null;
            }

            using (Stream stream = fileInfo.CreateReadStream())
            {
                // Buffer is returned to the pool in the middleware
                buffer = BufferDataPool.Rent((int)stream.Length);
                await stream.ReadAsync(buffer, 0, (int)stream.Length);
            }

            return buffer;
        }
    }
}