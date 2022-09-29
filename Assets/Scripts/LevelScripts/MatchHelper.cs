using System.Collections.Generic;
using System.Linq;

namespace Match3
{
    public class MatchHelper
    {
        public List<Cell> PossibleMatches { get; }

        public MatchHelper()
        {
            PossibleMatches = new List<Cell>();
        }

        public bool IsMatchPossible()
        {
            PossibleMatches.Clear();

            foreach (Cell cell in LevelManager.Singleton.AllCells)
            {
                for (int i = 0; i < 4; i++)
                {
                    Cell neighbour = null;
                    switch (i)
                    {
                        case 0:
                            neighbour = cell.Top;
                            break;
                        case 1:
                            neighbour = cell.Bot;
                            break;
                        case 2:
                            neighbour = cell.Left;
                            break;
                        case 3:
                            neighbour = cell.Right;
                            break;
                    }
                    Extensions.FindMatches(PossibleMatches, neighbour, cell);

                    if (PossibleMatches.Count <= 0) continue;
                    PossibleMatches.Add(cell);
                    return true;
                }
            }

            if (LevelManager.Singleton.AllCells.Where(cell => cell.HasChip()).Any(cell => cell.CurrentChip.Type == ChipType.None))
            {
                PossibleMatches.Add(LevelManager.Singleton.AllCells.FirstOrDefault(cell => cell.CurrentChip.Type == ChipType.None));
                return true;
            }

            return false;
        }
    }
}