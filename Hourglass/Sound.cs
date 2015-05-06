﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sound.cs" company="Chris Dziemborowicz">
//   Copyright (c) Chris Dziemborowicz. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hourglass
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// A sound that can be used to notify the user that a <see cref="Timer"/> has expired.
    /// </summary>
    public class Sound
    {
        /// <summary>
        /// The friendly name for this sound.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// A unique identifier for this sound.
        /// </summary>
        private readonly string identifier;

        /// <summary>
        /// A value indicating whether this sound is stored in the assembly.
        /// </summary>
        private readonly bool isBuiltIn;

        /// <summary>
        /// The path to the sound file.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// A method that returns a stream to the sound data.
        /// </summary>
        private readonly Func<UnmanagedMemoryStream> streamProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class for a sound stored in the file system.
        /// </summary>
        /// <param name="path">The path to the sound file.</param>
        public Sound(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            this.name = GetNameFromPath(path);
            this.identifier = GetIdentifierFromPath(path);
            this.isBuiltIn = false;
            this.path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sound"/> class for a sound stored in the assembly.
        /// </summary>
        /// <param name="name">The friendly name for the sound.</param>
        /// <param name="streamProvider">A method that returns a stream to the sound data.</param>
        public Sound(string name, Func<UnmanagedMemoryStream> streamProvider)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (streamProvider == null)
            {
                throw new ArgumentNullException("streamProvider");
            }

            this.name = name;
            this.identifier = "resource:" + name;
            this.isBuiltIn = true;
            this.streamProvider = streamProvider;
        }

        /// <summary>
        /// Gets the friendly name for this sound.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the unique identifier for this sound.
        /// </summary>
        public string Identifier
        {
            get { return this.identifier; }
        }

        /// <summary>
        /// Gets a value indicating whether this sound is stored in the assembly.
        /// </summary>
        public bool IsBuiltIn
        {
            get { return this.isBuiltIn; }
        }

        /// <summary>
        /// Gets the path to the sound file.
        /// </summary>
        public string Path
        {
            get { return this.path; }
        }

        /// <summary>
        /// Returns a <see cref="Sound"/> for the specified identifier, or <c>null</c> if the identifier is <c>null</c>
        /// or empty.
        /// </summary>
        /// <param name="identifier">The identifier for the sound.</param>
        /// <returns>A <see cref="Sound"/> for the specified identifier, or <c>null</c> if the identifier is
        /// <c>null</c> or empty.</returns>
        public static Sound FromIdentifier(string identifier)
        {
            return SoundManager.Instance.GetSoundOrDefault(identifier);
        }

        /// <summary>
        /// Returns a stream with the sound data.
        /// </summary>
        /// <returns>A stream with the sound data.</returns>
        public Stream GetStream()
        {
            return this.streamProvider != null
                ? (Stream)this.streamProvider()
                : new FileStream(this.path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// Returns the friendly name for a sound file.
        /// </summary>
        /// <param name="path">The path to the sound file.</param>
        /// <returns>The friendly name for a sound file.</returns>
        protected static string GetNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Returns the unique identifier for a sound file.
        /// </summary>
        /// <param name="path">The path to the sound file.</param>
        /// <returns>The unique identifier for a sound file.</returns>
        protected static string GetIdentifierFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            string appDirectory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
            string fullPath = System.IO.Path.GetFullPath(path);

            // Return a relative path if the sound is in or under the app directory, or otherwise return the full path
            return fullPath.StartsWith(appDirectory, StringComparison.OrdinalIgnoreCase)
                ? "file:." + fullPath.Substring(appDirectory.Length)
                : "file:" + path;
        }
    }
}
