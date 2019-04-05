/*
 * Author: Shon Verch
 * File Name: Program.cs
 * Project Name: BoggleChecker
 * Creation Date: 04/05/19
 * Modified Date: 04/05/19
 * Description: Determines whether a list of words exists on a boggle board.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            
            using (StreamReader reader = new StreamReader(new FileStream(filepath, FileMode.Open)))
            {
                int boardSize = int.Parse(reader.ReadLine());
                board = new Board(boardSize);

                for (int y = 0; y < boardSize; y++)
                {
                    string[] line = reader.ReadLine().Split(' ');
                    for (int x = 0; x < boardSize; x++)
                    {
                        board[x, y] = new Tile(line[x][0], x, y);
                    }
                }

                int dictionaryCount = int.Parse(reader.ReadLine());

                StringBuilder outputBuffer = new StringBuilder();
                int foundCount = 0;

                for (int i = 0; i < dictionaryCount; i++)
                {
                    string word = reader.ReadLine();
                    if (!FindWord(word)) continue;

                    foundCount += 1;
                    outputBuffer.AppendLine(word);
                }

                outputBuffer.Insert(0, string.Concat(foundCount, Environment.NewLine));
                string output = outputBuffer.ToString();

                Console.WriteLine(output);

                string filename = Path.GetFileNameWithoutExtension(filepath);
                File.WriteAllText(filepath.Replace(filename, "ShonVerch_" + filename), output);
            }
        }

        private static bool FindWord(string word, Tile currentTile = null, int remainingWordLength = 0, HashSet<Tile> visited = null)
        {
            if (visited == null)
            {
                visited = new HashSet<Tile>();
            }

            if (currentTile == null)
            {
                Tile startingTile = FindStartingTile(word);
                currentTile = startingTile ?? board[0, 0];
                if (startingTile != null)
                {
                    remainingWordLength += 1;
                }

                visited.Add(currentTile);
            }

            if (remainingWordLength == word.Length) return true;
            foreach (Tile neighbour in board.GetNeighbours(currentTile))
            {
                // The neighbour doesn't exist
                if (neighbour == null) continue;
                if (neighbour.Character != word[remainingWordLength] || visited.Contains(neighbour)) continue;

                visited.Add(neighbour);
                return FindWord(word, neighbour, remainingWordLength + 1, visited);
            }

            return false;
        }

        /// <summary>
        /// Retrives the first <see cref="Tile"/> whose character is the
        /// first character of the specified word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// A <see cref="Tile"/> value or <value>null</value>
        /// if no such <see cref="Tile"/> could be found.
        /// </returns>
        private static Tile FindStartingTile(string word)
        {
            for (int y = 0; y < board.Size; y++)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    if (board[x, y].Character == word[0]) return board[x, y];
                }
            }

            return null;
        }
    }
}
