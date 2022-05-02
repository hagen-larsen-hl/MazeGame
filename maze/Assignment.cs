using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace CS5410
{
    public class Assignment : Game
    {
        // Constants
        private int FRAME_OFFSET = 80;
        private Keys[] UP_KEYS = {Keys.W, Keys.Up, Keys.I};
        private Keys[] DOWN_KEYS = {Keys.S, Keys.Down, Keys.K};
        private Keys[] RIGHT_KEYS = {Keys.D, Keys.Right, Keys.L};
        private Keys[] LEFT_KEYS = {Keys.A, Keys.Left, Keys.J};
        
        // Helper Objects
        private Maze maze;
        private Stack<Cell> solutionStack;
        private Stack<Cell> breadcrumbs;
        private Character character;
        private KeyboardState prevState;
        private DateTime prevTime;
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;
        private SpriteFont m_font1;
        private SpriteFont m_font2;
        
        // Gameplay Variables
        private bool showMaze;
        private bool showSolution;
        private bool showHint;
        private bool showBreadcrumbs;
        private bool showScores;
        private bool showCredits;
        private int highFive;
        private int highTen;
        private int highFifteen;
        private int highTwenty;
        private bool win;
        
        // Textures
        private Texture2D m_b;
        private Texture2D m_brt;
        private Texture2D m_l;
        private Texture2D m_lb;
        private Texture2D m_lbr;
        private Texture2D m_lt;
        private Texture2D m_none;
        private Texture2D m_r;
        private Texture2D m_rb;
        private Texture2D m_rl;
        private Texture2D m_rt;
        private Texture2D m_rtl;
        private Texture2D m_t;
        private Texture2D m_tb;
        private Texture2D m_tlb;
        private Texture2D m_start;
        private Texture2D m_finish;
        private Texture2D m_character;
        private Texture2D m_pokeball;
        private Texture2D m_superball;
        private Texture2D m_boulderBadge;
        
        private float m_rotationRectangle = 0;
        
        public Assignment() 
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            prevTime = DateTime.Now;
            m_graphics.PreferredBackBufferWidth = 1024;
            m_graphics.PreferredBackBufferHeight = 768;
            m_graphics.ApplyChanges();
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load Fonts
            m_font1 = Content.Load<SpriteFont>("Fonts/DemoFont1");
            m_font2 = Content.Load<SpriteFont>("Fonts/DemoFont2");

            // Load Maze Tile PNGs
            m_b = Content.Load<Texture2D>("Images/b");
            m_brt = Content.Load<Texture2D>("Images/brt");
            m_l = Content.Load<Texture2D>("Images/l");
            m_lb = Content.Load<Texture2D>("Images/lb");
            m_lbr = Content.Load<Texture2D>("Images/lbr");
            m_lt = Content.Load<Texture2D>("Images/lt");
            m_none = Content.Load<Texture2D>("Images/none");
            m_r = Content.Load<Texture2D>("Images/r");
            m_rb = Content.Load<Texture2D>("Images/rb");
            m_rl = Content.Load<Texture2D>("Images/rl");
            m_rt = Content.Load<Texture2D>("Images/rt");
            m_rtl = Content.Load<Texture2D>("Images/rtl");
            m_t = Content.Load<Texture2D>("Images/t");
            m_tb = Content.Load<Texture2D>("Images/tb");
            m_tlb = Content.Load<Texture2D>("Images/tlb");
            m_start = Content.Load<Texture2D>("Images/start");
            m_finish = Content.Load<Texture2D>("Images/finish");
            m_pokeball = Content.Load<Texture2D>("Images/pokeball");
            m_character = Content.Load<Texture2D>("Images/character");
            m_superball = Content.Load<Texture2D>("Images/superball");
            m_boulderBadge = Content.Load<Texture2D>("Images/boulderBadge");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            // Track Elapsed Time
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - prevTime;
            if (showMaze)
            {
                maze.time += elapsed.TotalSeconds;
            }
            
            // Process Input
            ProcessInput();
            
            // Handle if player has won
            if (win)
            {
                if (maze.height == 5 && maze.score > highFive)
                {
                    highFive = maze.score;
                }
                if (maze.height == 10 && maze.score > highTen)
                {
                    highTen = maze.score;
                }
                if (maze.height == 15 && maze.score > highFifteen)
                {
                    highFifteen = maze.score;
                }
                if (maze.height == 20 && maze.score > highTwenty)
                {
                    highTwenty = maze.score;
                }
                showMaze = false;
                showSolution = false;
                showHint = false;
                showBreadcrumbs = false;
                showScores = true;
            }
            prevTime = now;
            
            base.Update(gameTime);
        }
        
        
        void ProcessInput()
        {
            KeyboardState state = Keyboard.GetState();
            foreach (Keys key in state.GetPressedKeys())
            {
                if (UP_KEYS.Contains(key) && !prevState.GetPressedKeys().Contains(key) && character.cell.topNeighbor != null)
                {
                    MoveCharacter(character.cell, character.cell.topNeighbor);
                }
                else if (DOWN_KEYS.Contains(key) && !prevState.GetPressedKeys().Contains(key) && character.cell.bottomNeighbor != null)
                {
                    MoveCharacter(character.cell, character.cell.bottomNeighbor);
                }
                else if (RIGHT_KEYS.Contains(key) && !prevState.GetPressedKeys().Contains(key) && character.cell.rightNeighbor != null)
                {
                    MoveCharacter(character.cell, character.cell.rightNeighbor);
                }
                else if (LEFT_KEYS.Contains(key) && !prevState.GetPressedKeys().Contains(key) && character.cell.leftNeighbor != null)
                {
                    MoveCharacter(character.cell, character.cell.leftNeighbor);
                }
                else if (Pressed(Keys.F1, prevState, state))
                {
                    GenerateMaze(5, 5);
                }
                else if (Pressed(Keys.F2, prevState, state))
                {
                    GenerateMaze(10, 10);
                }
                else if (Pressed(Keys.F3, prevState, state))
                {
                    GenerateMaze(15, 15);
                }
                else if (Pressed(Keys.F4, prevState, state))
                {
                    GenerateMaze(20, 20);
                }
                else if (Pressed(Keys.F5, prevState, state))
                {
                    showMaze = false;
                    showSolution = false;
                    showHint = false;
                    showBreadcrumbs = false;
                    showScores = true;
                    showCredits = false;
                }
                else if (Pressed(Keys.F6, prevState, state))
                {
                    win = false;
                    showMaze = false;
                    showSolution = false;
                    showBreadcrumbs = false;
                    showHint = false;
                    showScores = false;
                    showCredits = true;
                }
                else if (Pressed(Keys.H, prevState, state))
                {
                    showHint = !showHint;
                }
                else if (Pressed(Keys.P, prevState, state))
                {
                    showSolution = !showSolution;
                }
                else if (Pressed(Keys.B, prevState, state))
                {
                    showBreadcrumbs = !showBreadcrumbs;
                }
            }

            prevState = state;
        }

        // Simple helper method that determines if a key has been pressed since last previous keyboard state
        bool Pressed(Keys key, KeyboardState prevState, KeyboardState state)
        {
            return state.GetPressedKeys().Contains(key) && !prevState.GetPressedKeys().Contains(key);
        }

        // Helper method that moves character (logically) from given oldCell to newCell
        void MoveCharacter(Cell oldCell, Cell newCell)
        {
            breadcrumbs.Push(oldCell);
            
            // If player is near finish
            if (solutionStack.Count == 0)
            {
                if (!newCell.Equals(maze.finish)) 
                {
                    solutionStack.Push(oldCell);
                }
                else
                {
                    win = true;
                }
            }
            else
            {
                // Update shortest path and score
                if (solutionStack.Peek().Equals(newCell))
                {
                    if (!newCell.visited)
                    {
                        maze.score += 10;
                    }
                    solutionStack.Pop();
                }
                else
                {
                    if (!newCell.visited)
                    {
                        maze.score -= 2;
                    }
                    solutionStack.Push(oldCell);
                }
            }

            // Update character position
            character.cell = newCell;
            newCell.visited = true;
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            m_spriteBatch.Begin();
            DrawInstructions();
            if (showMaze) {DrawMaze();}
            if (showBreadcrumbs) {DrawBreadcrumbs();}
            if (showSolution) {DrawSolution();}
            if (showHint) {DrawHint();}
            if (showMaze) {DrawTile(character.m_character, character.cell);}
            if (showScores) {DrawScores();}
            if (showCredits) {DrawCredits();}
            m_spriteBatch.End();
            
            base.Draw(gameTime);
        }

        // Draws instructions that appear to right of maze
        void DrawInstructions()
        {
            m_spriteBatch.DrawString(m_font1, "Game Controls:", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F1: New Game 5x5", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 40), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F2: New Game 10x10", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 60), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F3: New Game 15x15", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 80), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F4: New Game 20x20", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 100), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F5: Display High Scores", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 120), Color.Black);
            m_spriteBatch.DrawString(m_font2, "F6: Display Credits", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 140), Color.Black);
            
            m_spriteBatch.DrawString(m_font1, "Character Controls:", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 180), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Move: arrow keys, WASD, or IJKL", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 220), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Breadcrumbs: b", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 240), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Path to Finish: p", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 260), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Hints: h", new Vector2(FRAME_OFFSET + 625, FRAME_OFFSET + 280), Color.Black);
        }
        
        // Calls drawTile (with appropriate Texture) for each cell in the map of the maze
        void DrawMaze()
        {
            for (int i = 0; i < maze.width; i++)
            {
                for (int j = 0; j < maze.height; j++)
                {
                    Cell cell = maze.cellMap[i, j];
                    bool topNeighbor = cell.topNeighbor != null;
                    bool bottomNeighbor = cell.bottomNeighbor != null;
                    bool rightNeighbor = cell.rightNeighbor != null;
                    bool leftNeighbor = cell.leftNeighbor != null;
                    
                    if (!topNeighbor)
                    {
                        if (!rightNeighbor)
                        {
                            if (!leftNeighbor)
                            {
                                DrawTile(m_rtl, cell);
                            }
                            else if (!bottomNeighbor)
                            {
                                DrawTile(m_brt, cell);
                            }
                            else
                            {
                                DrawTile(m_rt, cell);
                            }
                        }
                        else if (!leftNeighbor)
                        {
                            if (!bottomNeighbor)
                            {
                                DrawTile(m_tlb, cell);
                            }
                            else
                            {
                                DrawTile(m_lt, cell);
                            }
                        }
                        else if (!bottomNeighbor)
                        {
                            DrawTile(m_tb, cell);
                        }
                        else
                        {
                            DrawTile(m_t, cell);
                        }
                    }
                    else if (!leftNeighbor)
                    {
                        if (!rightNeighbor)
                        {
                            if (!bottomNeighbor)
                            {
                                DrawTile(m_lbr, cell);
                            }
                            else
                            {
                                DrawTile(m_rl, cell);
                            }
                        }
                        else if (!bottomNeighbor)
                        {
                            DrawTile(m_lb, cell);
                        }
                        else
                        {
                            DrawTile(m_l, cell);
                        }
                    }
                    else if (!rightNeighbor)
                    {
                        if (!bottomNeighbor)
                        {
                            DrawTile(m_rb, cell);
                        }
                        else
                        {
                            DrawTile(m_r, cell);
                        }
                    }
                    else if (!bottomNeighbor)
                    {
                        DrawTile(m_b, cell);
                    }
                    else
                    {
                        DrawTile(m_none, cell);
                    }
                }
            }
            m_spriteBatch.DrawString(m_font1, "Time: " + String.Format("{0:0}", maze.time) + " sec", new Vector2(FRAME_OFFSET, FRAME_OFFSET - 40), Color.Black);
            m_spriteBatch.DrawString(m_font1, "Score: " + maze.score, new Vector2(FRAME_OFFSET + 300, FRAME_OFFSET - 40), Color.Black);
            DrawTile(m_start, maze.start);
            DrawTile(m_finish, maze.finish);
        }

        // Draws a given texture in a given cell
        void DrawTile(Texture2D tile, Cell cell)
        {
            int proportion = 600 / maze.height;
            Rectangle rect = new Rectangle(FRAME_OFFSET + (proportion * cell.posX), FRAME_OFFSET + (proportion * cell.posY), proportion, proportion);
            m_spriteBatch.Draw(
                tile,
                rect,
                null,   // Drawing the whole texture, not a part
                Color.White,
                m_rotationRectangle,
                new Vector2(rect.Width / 2, rect.Height / 2),
                SpriteEffects.None,
                0);
        }

        // Draws a given texture in a given cell, but a fraction of the size (for breadcrumbs, hint, solution, etc.)
        void DrawSmallTile(Texture2D tile, Cell cell)
        {
            int proportion = 600 / maze.height;
            Rectangle rect = new Rectangle(FRAME_OFFSET + (proportion * cell.posX) + proportion / 4, FRAME_OFFSET + (proportion * cell.posY) + proportion / 4, proportion / 2, proportion / 2);
            m_spriteBatch.Draw(
                tile,
                rect,
                null,   // Drawing the whole texture, not a part
                Color.White,
                m_rotationRectangle,
                new Vector2(rect.Width / 2, rect.Height / 2),
                SpriteEffects.None,
                0);
        }

        // Draws the shortest path to finish
        void DrawSolution()
        {
            foreach (Cell cell in solutionStack)
            {
                DrawSmallTile(m_pokeball, cell);
            }
        }

        // Draws the history of character movement
        void DrawBreadcrumbs()
        {
            foreach (Cell cell in breadcrumbs)
            {
                DrawSmallTile(m_boulderBadge, cell);
            }
        }

        // Draws the suggested next move
        void DrawHint()
        {
            if (solutionStack.Count > 0)
            {
                Cell cell = solutionStack.Peek();
                DrawSmallTile(m_superball, cell);
            }
        }

        // Draws the high scores screen
        void DrawScores()
        {
            m_spriteBatch.DrawString(m_font1, "High Scores:", new Vector2(FRAME_OFFSET, FRAME_OFFSET), Color.Black);
            m_spriteBatch.DrawString(m_font1, "5x5: " + highFive, new Vector2(FRAME_OFFSET, FRAME_OFFSET + 40), Color.Black);
            m_spriteBatch.DrawString(m_font1, "10x10: " + highTen, new Vector2(FRAME_OFFSET, FRAME_OFFSET + 80), Color.Black);
            m_spriteBatch.DrawString(m_font1, "15x15: " + highFifteen, new Vector2(FRAME_OFFSET, FRAME_OFFSET + 120), Color.Black);
            m_spriteBatch.DrawString(m_font1, "20x20: " + highTwenty, new Vector2(FRAME_OFFSET, FRAME_OFFSET + 160), Color.Black);
        }

        // Draws the credits screen
        void DrawCredits()
        {
            m_spriteBatch.DrawString(m_font1, "Credits", new Vector2(FRAME_OFFSET, FRAME_OFFSET), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Game Creation: Hagen Larsen", new Vector2(FRAME_OFFSET, FRAME_OFFSET + 40), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Images: Definitely not Hagen Larsen", new Vector2(FRAME_OFFSET, FRAME_OFFSET + 80), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Maze Generation: Vojtech Jarnik (Prim's Algorithm)", new Vector2(FRAME_OFFSET, FRAME_OFFSET + 120), Color.Black);
            m_spriteBatch.DrawString(m_font2, "Fundamental C# Knowledge Needed: geeksforgeeks.org", new Vector2(FRAME_OFFSET, FRAME_OFFSET + 160), Color.Black);
        }

        // Accepts a Cell as input, returns a dictionary of visited neighbors
        // Input: Cell
        // Output: Dictionary (example: {'north': CellObj, 'east': CellObj}
        Dictionary<String, Cell> GetVisitedNeighbors(Cell cell)
        {
            Dictionary<string, Cell> dict = new Dictionary<string, Cell>();
            int posX = cell.posX;
            int posY = cell.posY;
            if (posY > 0 && maze.cellMap[posX, posY - 1].visited)
            {
                dict.Add("north", maze.cellMap[posX, posY - 1]);
            }

            if (posY < maze.height - 1 && maze.cellMap[posX, posY + 1].visited)
            {
                dict.Add("south", maze.cellMap[posX, posY + 1]);
            }

            if (posX > 0 && maze.cellMap[posX - 1, posY].visited)
            {
                dict.Add("east", maze.cellMap[posX - 1, posY]);
            }

            if (posX < maze.width - 1 && maze.cellMap[posX + 1, posY].visited)
            {
                dict.Add("west", maze.cellMap[posX + 1, posY]);
            }

            return dict;
        }

        // Accepts a Cell as input, returns a list of unvisited neighbors
        // Input: Cell
        // Output: List of Cells
        List<Cell> GetUnvisitedNeighbors(Cell cell)
        {
            List<Cell> list = new List<Cell>();
            int posX = cell.posX;
            int posY = cell.posY;
            if (posY > 0 && !maze.cellMap[posX, posY - 1].visited)
            {
                list.Add(maze.cellMap[posX, posY - 1]);
            }
            if (posY < maze.height - 1 && !maze.cellMap[posX, posY + 1].visited)
            {
                list.Add(maze.cellMap[posX, posY + 1]);
            }
            if (posX > 0 && !maze.cellMap[posX - 1, posY].visited)
            {
                list.Add(maze.cellMap[posX - 1, posY]);
            }
            if (posX < maze.width - 1 && !maze.cellMap[posX + 1, posY].visited)
            {
                list.Add(maze.cellMap[posX + 1, posY]);
            }
            
            return list;
        }

        // Creates relationships between two cells that can be visited from one another
        // Input: Key-Value pair identifying a cell and its relation to other given cell
        //        Cell
        // Output: None
        void ConnectCells(KeyValuePair<String, Cell> connection, Cell cell)
        {
            if (connection.Key == "north")
            {
                cell.topNeighbor = connection.Value;
                connection.Value.bottomNeighbor = cell;
            }
            else if (connection.Key == "south")
            {
                cell.bottomNeighbor = connection.Value;
                connection.Value.topNeighbor = cell;
            }
            else if (connection.Key == "west")
            {
                cell.rightNeighbor = connection.Value;
                connection.Value.leftNeighbor = cell;
            }
            else
            {
                cell.leftNeighbor = connection.Value;
                connection.Value.rightNeighbor = cell;
            }
        }
        
        // Maze Generation algorithm using Prim's algorithm
        void GenerateMaze(int sizeX, int sizeY)
        {
            win = false;
            showCredits = false;
            showScores = false;
            showMaze = true;
            showSolution = false;
            showHint = false;
            showBreadcrumbs = false;
            solutionStack = new Stack<Cell>();
            breadcrumbs = new Stack<Cell>();
            maze = new Maze(sizeX, sizeY);
            // Pick a cell, mark as part of maze
            Random random = new Random();
            Cell cell = maze.start;
            cell.visited = true;
            List<Cell> eligibleCells = new List<Cell>();
            eligibleCells.Add(cell);
            while (eligibleCells.Count > 0)
            {
                // Select a random cell from the list
                cell = eligibleCells[random.Next(eligibleCells.Count)];
                cell.visited = true;
                // Find any neighboring cells that are already part of the maze
                Dictionary<String, Cell> neighbors = GetVisitedNeighbors(cell);
                if (neighbors.Count > 0)
                {
                    // Randomly pick a neighbor that is part of the maze, make the connection between the two cells
                    ConnectCells(neighbors.ElementAt(random.Next(neighbors.Count)), cell);
                }
                // Add unvisited neighbors to the list of eligible cells to be used next
                List<Cell> addCells = GetUnvisitedNeighbors(cell);
                foreach (Cell addCell in addCells)
                {
                    if (!eligibleCells.Contains(addCell))
                    {
                        eligibleCells.Add(addCell);
                    }
                }
                // Remove the current cell from the list of eligible cells, as it is now part of the maze
                eligibleCells.Remove(cell);
            }
            maze.finish = cell;
            character = new Character(maze.start, m_character);
            SolveMaze();
            ComposeSolution();
        }

        // BFS to find a solution (shortest solution) to the generated maze
        void SolveMaze()
        {
            Queue<Cell> queue = new Queue<Cell>();
            queue.Enqueue(maze.start);
            while (queue.Count > 0)
            {
                Cell cell = queue.Dequeue();
                if (cell.Equals(maze.finish))
                {
                    return;
                }

                if (cell.bottomNeighbor != null && cell.bottomNeighbor.visited)
                {
                    cell.bottomNeighbor.visited = false;
                    cell.bottomNeighbor.prior = cell;
                    queue.Enqueue(cell.bottomNeighbor);
                }
                if (cell.topNeighbor != null && cell.topNeighbor.visited)
                {
                    cell.topNeighbor.visited = false;
                    cell.topNeighbor.prior = cell;
                    queue.Enqueue(cell.topNeighbor);
                }
                if (cell.rightNeighbor != null && cell.rightNeighbor.visited)
                {
                    cell.rightNeighbor.visited = false;
                    cell.rightNeighbor.prior = cell;
                    queue.Enqueue(cell.rightNeighbor);
                }
                if (cell.leftNeighbor != null && cell.leftNeighbor.visited)
                {
                    cell.leftNeighbor.visited = false;
                    cell.leftNeighbor.prior = cell;
                    queue.Enqueue(cell.leftNeighbor);
                }
            }
            
            // Reset cell 'visited' variables
            for (int i = 0; i < maze.width; i++)
            {
                for (int j = 0; j < maze.height; j++)
                {
                    maze.cellMap[i, j].visited = false;
                }
            }
        }

        // Composes the a solution stack data structure
        void ComposeSolution()
        {
            Cell cell = maze.finish;
            while (!cell.Equals(maze.start))
            {
                if (!cell.Equals(maze.finish))
                {
                    solutionStack.Push(cell);
                }
                cell = cell.prior;
            }
        }
    }
}
