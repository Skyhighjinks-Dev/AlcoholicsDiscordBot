namespace AlcoholicsDiscordBot.Disc.Games;

public class WordSearch
{
  private static readonly Random _random = new Random();

  // Letter frequency weights based on English language frequency
  // Higher numbers = more likely to appear
  private static readonly Dictionary<char, int> LetterWeights = new Dictionary<char, int>
    {
        {'E', 9}, {'T', 8}, {'A', 8}, {'O', 9}, {'I', 9}, {'N', 8},
        {'S', 8}, {'H', 7}, {'R', 7}, {'D', 6}, {'L', 6}, {'C', 5},
        {'U', 5}, {'M', 5}, {'W', 5}, {'F', 4}, {'G', 4}, {'Y', 4},
        {'P', 4}, {'B', 3}, {'V', 3}, {'K', 3}, {'J', 2}, {'X', 2},
        {'Q', 2}, {'Z', 2}
    };

  private static readonly char[] WeightedLetterPool;

  static WordSearch()
  {
    // Build a pool of letters based on weights
    var pool = new List<char>();
    foreach (var kvp in LetterWeights)
    {
      for (int i = 0; i < kvp.Value; i++)
      {
        pool.Add(kvp.Key);
      }
    }
    WeightedLetterPool = pool.ToArray();
  }

  private enum Direction
  {
    Horizontal,
    Vertical,
    DiagonalDown,
    DiagonalUp
  }

  private struct Grid
  {
    public List<GridWord> Words { get; set; }
    public byte GridSize { get; set; }
    public char[,] Cells { get; set; }

    public Grid(byte nGridSize)
    {
      this.Words = new List<GridWord>();
      this.GridSize = nGridSize;
      this.Cells = new char[nGridSize, nGridSize];

      // Initialize grid with empty cells
      for (int x = 0; x < nGridSize; x++)
        for (int y = 0; y < nGridSize; y++)
          this.Cells[x, y] = '\0';
    }

    public void FillEmptyCells()
    {
      for (int x = 0; x < this.GridSize; x++)
      {
        for (int y = 0; y < this.GridSize; y++)
        {
          if (Cells[x, y] == '\0')
          {
            // Use weighted random letter selection
            Cells[x, y] = GetRandomWeightedLetter();
          }
        }
      }
    }

    private static char GetRandomWeightedLetter()
    {
      return WeightedLetterPool[_random.Next(WeightedLetterPool.Length)];
    }
  }

  private struct GridWord
  {
    public GridCharacter[] Characters { get; set; }
    public int Length { get => Characters.Length; }
    public string Word { get => new string(Characters.Select(c => c.Character).ToArray()); }
    public int StartX { get => Characters[0].X; }
    public int StartY { get => Characters[0].Y; }
    public int EndX { get => Characters[Length - 1].X; }
    public int EndY { get => Characters[Length - 1].Y; }
    public Direction Direction { get; set; }

    public GridWord(GridCharacter[] nCharacters, Direction direction)
    {
      this.Characters = nCharacters;
      this.Direction = direction;
    }
  }

  private struct GridCharacter
  {
    public int X { get; set; }
    public int Y { get; set; }
    public char Character { get; set; }

    public GridCharacter(int nX, int nY, char nCharacter)
    {
      this.X = nX;
      this.Y = nY;
      // Convert to uppercase and validate
      char upperChar = char.ToUpper(nCharacter);
      this.Character = (upperChar >= 'A' && upperChar <= 'Z') ? upperChar :
          throw new ArgumentException($"Invalid character: {nCharacter}");
    }
  }

  private byte MaxWordLength { get; set; }
  private byte GridSize { get; set; }
  private Grid CurrentGrid { get; set; }

  public WordSearch(byte nMaxWordLength = 10, byte nGridSize = 10)
  {
    if (nMaxWordLength > nGridSize)
      nMaxWordLength = nGridSize;

    this.MaxWordLength = nMaxWordLength;
    this.GridSize = nGridSize;
  }

  public (char[,] grid, List<string> placedWords) GeneratePuzzle(List<string> words)
  {
    // Filter and prepare words
    var validWords = words
        .Where(w => w.Length <= MaxWordLength && w.Length > 0)
        .Select(w => w.ToUpper())
        .OrderByDescending(w => w.Length) // Place longer words first
        .ToList();

    CurrentGrid = new Grid(GridSize);
    List<string> successfullyPlacedWords = new List<string>();

    foreach (var word in validWords)
    {
      if (TryPlaceWord(word))
      {
        successfullyPlacedWords.Add(word);
      }
    }

    // Fill remaining empty cells with random letters
    CurrentGrid.FillEmptyCells();

    return (CurrentGrid.Cells, successfullyPlacedWords);
  }

  private bool TryPlaceWord(string word)
  {
    // Get all possible directions in random order
    var directions = Enum.GetValues<Direction>().OrderBy(x => _random.Next()).ToList();

    // Try multiple random starting positions
    for (int attempt = 0; attempt < 100; attempt++)
    {
      int startX = _random.Next(GridSize);
      int startY = _random.Next(GridSize);

      foreach (var direction in directions)
      {
        if (CanPlaceWord(word, startX, startY, direction))
        {
          PlaceWord(word, startX, startY, direction);
          return true;
        }
      }
    }

    return false;
  }

  private bool CanPlaceWord(string word, int startX, int startY, Direction direction)
  {
    var (dx, dy) = GetDirectionOffsets(direction);

    // Check if word fits within grid boundaries
    int endX = startX + dx * (word.Length - 1);
    int endY = startY + dy * (word.Length - 1);

    if (endX < 0 || endX >= GridSize || endY < 0 || endY >= GridSize)
      return false;

    // Check each position for conflicts
    for (int i = 0; i < word.Length; i++)
    {
      int x = startX + dx * i;
      int y = startY + dy * i;

      // Cell must be empty or contain the same letter
      if (CurrentGrid.Cells[x, y] != '\0' && CurrentGrid.Cells[x, y] != word[i])
        return false;
    }

    return true;
  }

  private void PlaceWord(string word, int startX, int startY, Direction direction)
  {
    var (dx, dy) = GetDirectionOffsets(direction);
    var characters = new GridCharacter[word.Length];

    for (int i = 0; i < word.Length; i++)
    {
      int x = startX + dx * i;
      int y = startY + dy * i;

      CurrentGrid.Cells[x, y] = word[i];
      characters[i] = new GridCharacter(x, y, word[i]);
    }

    CurrentGrid.Words.Add(new GridWord(characters, direction));
  }

  private (int dx, int dy) GetDirectionOffsets(Direction direction)
  {
    return direction switch
    {
      Direction.Horizontal => (1, 0),
      Direction.Vertical => (0, 1),
      Direction.DiagonalDown => (1, 1),
      Direction.DiagonalUp => (1, -1),
      _ => (0, 0)
    };
  }

  // Helper method to get the grid as a string for testing
  public string GetGridAsString()
  {
    if (CurrentGrid.Cells == null)
      return "Grid not generated yet";

    var result = new System.Text.StringBuilder();

    for (int y = 0; y < GridSize; y++)
    {
      for (int x = 0; x < GridSize; x++)
      {
        result.Append(CurrentGrid.Cells[x, y]);
        result.Append(' ');
      }
      result.AppendLine();
    }

    return result.ToString();
  }
}