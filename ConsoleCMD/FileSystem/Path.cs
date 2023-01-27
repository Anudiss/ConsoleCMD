using System;
using System.Collections.Generic;
using SysIO = System.IO;

namespace ConsoleCMD.FileSystem
{
    public class Path
    {
        public PathKind Kind { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                Slices = value.Split(new char[] { SysIO.Path.DirectorySeparatorChar },
                    StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private IEnumerable<string> _slices;
        public IEnumerable<string> Slices
        {

            get => _slices;
            set
            {
                _slices = value;
                _value = string.Join(SysIO.Path.DirectorySeparatorChar, value);
            }
        }

        public Path() { }

        public Path(string value)
            => Value = value;

        public Path(IEnumerable<string> slices)
            => Slices = slices;

        public override bool Equals(object obj)
        {
            if (obj is Path path == false)
                return false;
            return this.Kind == path.Kind && this.Value == path.Value;
        }

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator== (Path p1, Path p2)
            => p1.Kind == p2.Kind && p1.Value == p2.Value;

        public static bool operator!= (Path p1, Path p2)
            => p1.Kind != p2.Kind || p1.Value != p2.Value;
    }
}