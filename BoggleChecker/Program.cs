/*
 * Author: Shon Verch
 * File Name: Program.cs
 * Project Name: BoggleChecker
 * Creation Date: 04/05/19
 * Modified Date: 04/09/19
 * Description: Determines whether a list of words exists on a boggle board.
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace BoggleChecker
{
    /// <summary>
    /// A single tile on the board.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// The character that this <see cref="Tile"/> represents.
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// The x-coordinate of this <see cref="Tile"/> on the board.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The y-coordinate of this <see cref="Tile"/> on the board.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes a new <see cref="Tile"/>.
        /// </summary>
        /// <param name="character">The character that this <see cref="Tile"/> represents.</param>
        /// <param name="x">The x-coordinate of this <see cref="Tile"/> on the board.</param>
        /// <param name="y">The y-coordinate of this <see cref="Tile"/> on the board.</param>
        public Tile(char character, int x, int y)
        {
            Character = character;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// An N x N boggle-board consisting of tiles.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Gets or sets a <see cref="Tile"/> at the specified (x, y) coordinate.
        /// </summary>
        /// <param name="x">The x-coordinate of the <see cref="Tile"/>.</param>
        /// <param name="y">The y-coordinate of the <see cref="Tile"/>.</param>
        /// <returns>
        /// The <see cref="Tile"/> at the specified coordinate or
        /// <value>null</value> if the coordinate is out-of-bounds.
        /// </returns>
        public Tile this[int x, int y]
        {
            get => GetTileAt(x, y);
            set => tiles[x, y] = value;
        }

        /// <summary>
        /// The size of this <see cref="Board"/>.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// The tiles that compose this <see cref="Board"/>.
        /// </summary>
        private readonly Tile[,] tiles;

        /// <summary>
        /// Initialize a new <see cref="Board"/>.
        /// </summary>
        /// <param name="size">The size (rows and columns) of this <see cref="Board"/>.</param>
        public Board(int size)
        {
            Size = size;
            tiles = new Tile[size, size];
        }

        /// <summary>
        /// Retrieves the neighbours of the specified <see cref="Tile"/>.
        /// </summary>
        /// <remarks>
        /// The array consists of the 8 primary ordinal directions read clockwise:
        /// north, north-east, east, south-east, south, south-west, west, and north-west. 
        /// </remarks>
        /// <param name="tile">The <see cref="Tile"/> to get neighbours around.</param>
        /// <returns>
        /// An array of <see cref="Tile"/> consisting of the 8 neighbours around the specified <see cref="Tile"/>.
        /// If the i-th element is <value>null</value>, a neighbour couldn't be found at the i-th ordinal direction.
        /// </returns>
        public Tile[] GetNeighbours(Tile tile)
        {
            Tile[] neighbours = new Tile[8];

            // North
            neighbours[0] = GetTileAt(tile.X, tile.Y + 1);
            // North-east
            neighbours[1] = GetTileAt(tile.X + 1, tile.Y + 1);
            //  East
            neighbours[2] = GetTileAt(tile.X + 1, tile.Y);
            // South-east
            neighbours[3] = GetTileAt(tile.X + 1, tile.Y - 1);
            // South
            neighbours[4] = GetTileAt(tile.X, tile.Y - 1);
            // South-west
            neighbours[5] = GetTileAt(tile.X - 1, tile.Y - 1);
            // West
            neighbours[6] = GetTileAt(tile.X - 1, tile.Y);
            // North-west
            neighbours[7] = GetTileAt(tile.X - 1, tile.Y + 1);

            return neighbours;
        }

        /// <summary>
        /// Retrieves a <see cref="Tile"/> at the specified (x, y) coordinate.
        /// </summary>
        /// <param name="x">The x-coordinate of the <see cref="Tile"/>.</param>
        /// <param name="y">The y-coordinate of the <see cref="Tile"/>.</param>
        /// <returns>
        /// The <see cref="Tile"/> at the specified coordinate or
        /// <value>null</value> if the coordinate is out-of-bounds.
        /// </returns>
        public Tile GetTileAt(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size) return null;
            return tiles[x, y];
        }
    }

    /// <summary>
    /// Determines whether a list of words exists on a boggle board.
    /// </summary>
    internal class Program
    {
        private static Board board;

        /// <summary>
        /// Entry-point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        private static void Main(string[] args)
        {
            Console.WriteLine("What is the input filename?");
            string filepath = Console.ReadLine();

            if (!File.Exists(filepath)) return;
            
            // Open and read the input data file.
            using (StreamReader reader = new StreamReader(new FileStream(filepath, FileMode.Open)))
            {
                int boardSize = int.Parse(reader.ReadLine());
                board = new Board(boardSize);

                // Initialize the board.
                for (int y = 0; y < boardSize; y++)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    for (int x = 0; x < boardSize; x++)
                    {
                        board[x, y] = new Tile(line[x][0], x, y);
                    }
                }

                int dictionaryCount = int.Parse(reader.ReadLine());
                
                // A set to track duplicate word queries
                HashSet<string> resultCache = new HashSet<string>();
                for (int i = 0; i < dictionaryCount; i++)
                {
                    string word = reader.ReadLine();
                    if (resultCache.Contains(word) || !FindWord(word)) continue;
                    resultCache.Add(word);
                }

                // Build the output string
                string output = $"{resultCache.Count}\n";
                foreach (string word in resultCache)
                {
                    output += $"{word}\n";
                }

                // Write the output to console and file.
                Console.WriteLine(output);

                // Extract the filename without the extension and then
                // replace the filename in the filepath with the output
                // filename (e.g. "ShonVerch_inputFilename")
                string filename = Path.GetFileNameWithoutExtension(filepath);
                File.WriteAllText(filepath.Replace(filename, "ShonVerch_" + filename), output);

                Console.ReadLine();
            }
        }

        /// <summary>
        /// Determine whether the specified word exists on the board.
        /// </summary>
        /// <param name="word">The word to query/</param>
        /// <returns>
        /// A boolean value indicating whether the specified word
        /// exists on the board: true if it does, false otherwise.
        /// </returns>
        private static bool FindWord(string word)
        {
            // Find the tiles on the board whose
            // character is the same as the first character
            // of the specified word.
            for (int y = 0; y < board.Size; y++)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    Tile currentTile = board[x, y];
                    if (currentTile.Character != word[0]) continue;

                    // Create the visited set and add the current
                    // tile to it since we are starting on this tile;
                    // thefore, we cannot visit it again.
                    HashSet<Tile> visited = new HashSet<Tile>
                    {
                        currentTile
                    };

                    if (Search(word, currentTile, 1, visited)) return true;
                }
            }

            // If we couldn't find any tile that starts with the same
            // character as our word, start from the first tile (0, 0).
            return Search(word, board[0, 0], 0, new HashSet<Tile>());
        }

        /// <summary>
        /// Search for the specified word, starting from the specified tile.
        /// </summary>
        /// <param name="word">The <see cref="string"/> to search for.</param>
        /// <param name="currentTile">The <see cref="Tile"/> to start the search at.</param>
        /// <param name="remainingWordLength">The remaining length of the search word.</param>
        /// <param name="visited">A <see cref="HashSet{T}"/> containing the <see cref="Tile"/>s that have been visited.</param>
        /// <returns>A boolean value indicating whether the specified word was found.</returns>
        private static bool Search(string word, Tile currentTile, int remainingWordLength, HashSet<Tile> visited)
        {
            // When we have reached a search depth that is the length of
            // the word we are searching for, we know that we have found
            // the word: return true.
            if (remainingWordLength == word.Length) return true;

            // Check each ordinal neighbour to the current tile
            foreach (Tile neighbour in board.GetNeighbours(currentTile))
            {
                /*
                 * Skip this neighbour if:
                 *  a) it doesn't exist (i.e. is null)
                 *  b) its character value is not equal to the character
                 *     that we are currently looking for
                 *  c) we have already visited it
                 */
                if (neighbour == null) continue;
                if (neighbour.Character != word[remainingWordLength] || visited.Contains(neighbour)) continue;
                
                // Shallow copy of the visited collection for the next recursive branch
                // We can't share the same visited set since multiple branches visited
                // different tiles.
                HashSet<Tile> newVisited = new HashSet<Tile>(visited)
                {
                    neighbour
                };

                // If we found something in the next recursive branch, return true in this one.
                if (Search(word, neighbour, remainingWordLength + 1, newVisited)) return true;
            }

            // We didn't find anything in any of the ordinal neighbours so return false.
            return false;
        }
    }
}
