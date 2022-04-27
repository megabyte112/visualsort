// standard system libraries
using System;

// allows linq to help shuffle arrays
using System.Linq;

// xna/monogame framework
using Microsoft.Xna.Framework;

// rendering
using Microsoft.Xna.Framework.Graphics;

// input
using Microsoft.Xna.Framework.Input;


// begin program here
namespace visualsort
{
    public class visualsort : Game
    {
        // monogame rendering stuff
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // random number generation
        static readonly Random r = new Random();

        // timing
        static bool doFrameLimit = true; // should frme limit be on
        static readonly double framespersecond = 600d; // frames per second
        static readonly int delay = 0;    // delay in frames

        // array
        static readonly int size = 128;   // number of elements in array
        static readonly int width = 8; // width of each bar in pixels
        int[] list = new int[size];

        // status:
        // 0: sorted    1: shuffled
        // 2: bubblesort    3: initializing
        static int status = 3;

        // allows delays without freezing the program
        static int delayframes = 0;

        // index of item being compared
        static int j = 0;

        // number of consecutive non-swaps
        static int run = 0;

        // max length to check
        static int maxlength = size - 1;

        // input
        Keys[] keypresses;
        Keys[] oldKeypresses;
        Keys[] changedKeys;

        // monogame app config
        public visualsort()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;

            // lock to 60fps for consistency across different hardware
            if (doFrameLimit)
            {
                IsFixedTimeStep = true;
                TargetElapsedTime = TimeSpan.FromSeconds(1d / framespersecond);
            }
        }

        // run on startup
        protected override void Initialize()
        {
            // set window size
            _graphics.PreferredBackBufferWidth = size*width;
            _graphics.PreferredBackBufferHeight = size*(width/2);
            _graphics.ApplyChanges();

            // add numbers to the list
            for (int i = 1; i <= size; i++) list[i - 1] = i;
            status = 0;

            base.Initialize();
        }

        // initialise spritebatch for rendering
        protected override void LoadContent() { _spriteBatch = new SpriteBatch(GraphicsDevice); }

        // runs each frame
        protected override void Update(GameTime gameTime)
        {
            // input
            oldKeypresses = keypresses;
            keypresses = Keyboard.GetState().GetPressedKeys();

            // make array of keypress changed
            if (oldKeypresses != null) changedKeys = keypresses.Except(oldKeypresses).ToArray();
            else changedKeys = keypresses;

            // iterate through changed keys
            foreach (var key in changedKeys)
            {
                // this is way too overkill for this app,
                // using 'if' statements would be fine.
                // it's a good habit for other apps that use more keys.
                switch (key)
                {
                    // toggle shuffle and sort
                    case Keys.Space:
                        if (status == 0)
                        {
                            list = ShuffleList(list);
                            j = 0;
                            maxlength=size - 1;
                            run=0;
                            status = 1;
                        }
                        else if (status == 1) status = 2;
                        else if (status == 2) status = 1;
                        break;
                    // exit
                    case Keys.Escape:
                        Exit();
                        break;
                    case Keys.Enter:
                        // step once
                        if (status == 1)
                        {
                            list = BubbleSortStep(list);
                            if (IsSorted(list)) status = 0;
                        }
                        break;
                }
            }

            // run bubblesort
            if (status == 2 && delayframes == 0)
            {
                // run bubblesort algorithm once
                list = BubbleSortStep(list);

                // pause until delay is over
                delayframes = delay;

                // check if sorted
                if (IsSorted(list)) status = 0;
            }
            else if (delayframes > 0) delayframes--;
            base.Update(gameTime);
        }
        
        // rendering
        protected override void Draw(GameTime gameTime)
        {
            // clear the screen
            GraphicsDevice.Clear(Color.Black);

            // begin drawing stuff
            _spriteBatch.Begin();

            // for every item in the list, create and draw a rectangle
            for (int i = 0; i < size; i++)
            {
                // make a new rectangle texture
                Texture2D rect = new Texture2D(_graphics.GraphicsDevice, width, (width/2) * list[i]);

                // give each pixel a colour
                Color[] data = new Color[width * (width/2) * list[i]];
                if (i==j && !IsSorted(list)) { for (int k = 0; k < data.Length; k++) data[k] = Color.Red; }
                else { for (int k = 0; k < data.Length; k++) data[k] = Color.White; }
                rect.SetData(data);

                // calculate the position to draw
                Vector2 pos = new Vector2(i * width, _graphics.PreferredBackBufferHeight - ((width/2) * list[i]));

                // draw the rectangle
                _spriteBatch.Draw(rect, pos, Color.White);
            }

            // stop drawing stuff
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // shuffles the list
        // i have no idea how, it just does
        int[] ShuffleList(int[] list) { return list.OrderBy(list => r.Next()).ToArray(); }

        // check if the list is sorted
        bool IsSorted(int[] list)
        {
            for (int i = 0; i < list.Length - 1; i++)
            {
                if (list[i] > list[i + 1]) return false;
            }
            return true;
        }

        // bubble sort
        static int[] BubbleSortStep(int[] list)
        {
            if (j == maxlength)
            {
                j = 0;
                maxlength-=run+1;
            }
            if (list[j] > list[j + 1])
            {
                int temp = list[j];
                list[j] = list[j + 1];
                list[j + 1] = temp;
                run=0;
            }
            else
            {
                run++;
            }
            j++;
            return list;
        }
    }
}
