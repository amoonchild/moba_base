

namespace PathFinder
{
    /// <summary>
	/// map node
	/// </summary>
    public interface INode
    {
        int Index
        {
            get;
        }

        int X
        {
            get;
        }

        int Y
        {
            get;
        }

        bool IsWalkable
        {
            get;
            set;
        }

        int H
        {
            get;
            set;
        }

        int G
        {
            get;
            set;
        }

        int F
        {
            get;
        }

        INode ParentNode
        {
            get;
            set;
        }
    }
}