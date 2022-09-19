using System.Collections.Generic;
using System.Linq;

namespace Match3
{
    public class MatchHelper
    {
        private readonly Cell[] _allCells;
        private readonly List<Cell> _possibleMatch;
        public MatchHelper(Cell[] cells)
        {
            _allCells = cells;
            _possibleMatch = new List<Cell>();
        }

        public void Execute()
        {
            _possibleMatch.Clear();

            if (_allCells.Any(z => z.CurrentChip.Type == ChipType.None))
            {
                _allCells.FirstOrDefault(z => z.CurrentChip.Type == ChipType.None)?.HighlightCell();
                return;
            }

            foreach (Cell cell in _allCells)
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
                    Extensions.FindMatches(_possibleMatch, neighbour, cell);

                    if (_possibleMatch.Count <= 0) continue;
                    foreach (Cell possibleCell in _possibleMatch)
                        possibleCell.HighlightCell();

                    cell.HighlightCell();
                    return;
                }
            }
        }
    }
}