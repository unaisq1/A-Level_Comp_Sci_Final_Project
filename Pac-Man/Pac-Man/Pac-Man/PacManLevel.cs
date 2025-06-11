using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace Pac_Man
{
    public partial class PacManLevel : Form
    {
        //Boolean Values activiated when the player moves based on movement keys
        private bool moveLeft;
        private bool moveRight;
        private bool moveUp;
        private bool moveDown;

        //Boolean Values activiated when Dinky moves based on specific picturebox tiles
        private bool dinkyLeft;
        private bool dinkyRight;
        private bool dinkyUp;
        private bool dinkyDown;

        //Boolean Values based on whether the player can move in certain directions in certain situations
        private bool ableToUp;
        private bool ableToDown;
        private bool ableToLeft;
        private bool ableToRight;

        //Boolean Value based on whether the player can move at all in certain situations
        private bool ableToMove = false;

        private string lastAction; //String value based on the player's last action at an intersection, it changes whenever the player reaches an intersection
        private string veryLastAction; //String value based on the player's very last action performed from the controls

        private string lastDinkyAction; //String value based on Dinky's last action at a picturebox tile, changes whenever Dinky reaches a new picturebox tile in its path

        private int startCount = 0; //Count that increases to 5 whenever a game starts, to recreate the beginning of a Pac-Man game
        private int endCount = 0; //Count that increases to 5 when the player loses all lives, to recreate the ending of a Pac-Man game
        private int resetTimerCount = 0; //Count that increases to 3 when the player loses a life, to create the loss of a life in a Pac-Man game
        private int powerTimerCount = 0; //Count that increases to 10 whenever a power pellet is collected, to recreate the duration of a power pellet
        private int deathTimerCount = 0;

        //Count that increases to 5 or 10 (depending on the effect) whenever a support/danger/random pellet is collected, to create the duration of those pellets when collected
        private int supportTimerCount = 0; 
        private int dangerTimerCount = 0; 
        private int randomTimerCount = 0; 

        //Boolean Value based on whether the player can pause in certain situations (such as the beginning of a game, when the player loses a life, or when the player loses all lives) 
        private bool ableToPause = false;

        //Integer Value used to count how many pellets have been collected, if they reach 242, the level is complete and a new level starts. The count resets
        private int itemsCollected = 0;

        //Integer Value of the movement speed of the player and Dinky. The maze is built entirely based on the default speed value of 12, changing this value may break the system of
        //how movement works due to the sizes of the pathways and intersections.
        public int speed = 12;
        
        public static int level = 1; //Count of the level being played, increases when the player collects all pellets
        public static int score = 0; //Measure of the amount of points a player has scored in a single game. The score increases when a player collects pellets, fruit, or eats ghosts
        private int multiplier = 1; //Multiplies the values of the items that grant points to the player. Default value is 1 but can increase to 2, 3 or 4 when a support effect takes place.
        private bool ableToScore = true; //Allows the player to score points, when deactivated, the items that usually grant points don't for 10 seconds
        private int lives = 2; //The amount of lives a player has in a single game. Increases when a player collects 5000 points, where the oneUp tracker resets after 5000 points are collected. Decreases
        //when the player touches a ghost. When it reaches below 0, the game ends.
        private int oneUp = 0; //Tracks the amount of points a player has until it reaches 5000. When it reaches 5000, the lives increase by 1 and the tracker resets back to 0.

        private List<string> moveList = new List<string>(); //List that contains 1-2 string values for the directions the player has taken. Used in intersections, where the [0] value is used to determine
        //which direction the player will go in at the intersection
        private List<string> actionList = new List<string>(); //Partially unused list, originally intended to make Ms. Pac-Man's animations more natural when going Up or Down

        //Boolean Values used to register which buttons were pressed, used for sprite animation directions
        private bool leftButtonPress;
        private bool rightButtonPress;
        private bool upButtonPress;
        private bool downButtonPress;

        private int fruitAppear; //Value used to indicate whether a fruit item can appear, and to make sure only one can appear at a time
        private int nextFruit; //Random value used to decide which specific fruit item appears

        //Values used to indicate whether a special pellet has been collected. This is used so the timer properly resets when more than one special pellet is collected (for example, if a power pellet is
        //collected and the timer is already counting up, if another power pellet is collected then the timer should reset back to 0 (to extend the duration)
        private int powerPelletCollected = 0;
        private int supportPelletCollected = 0;
        private int dangerPelletCollected = 0;
        private int randomPelletCollected = 0;

        //Boolean Values for when the game is paused and the timers need to be paused as well. Checks whether the timers were paused when clicking the resume button, that way they continue only
        //if the game is resumed and in progress
        private bool PowerPelletTimerPaused = false;
        private bool SupportPelletTimerPaused = false;
        private bool DangerPelletTimerPaused = false;
        private bool RandomPelletTimerPaused = false;

        private bool invincible = false; //Boolean Value to track whether the player has eaten a Power Pellet and is therefore invincible or not
        private bool invisible = false; //Boolean value to track whether the player has eaten a Support Pellet and is invisible from one of the effects or not
        
        private int supportEffect = 0; //Random integer value that sets the specific effect that a player gains when they've eaten a support pellet
        private int supportEffectDuration = 0;
        private bool supportEffectAvailable = false;

        private int dangerEffect = 0; //Random integer value that sets the specific effect that a player gains when they've eaten a danger pellet
        private int dangerEffectDuration = 0;
        private bool dangerEffectAvailable = false;

        private int randomEffect = 0; //Random integer value that sets the specific effect that a player gains when they've eaten a random pellet
        private int randomEffectDuration = 0;
        private bool randomEffectAvailable = false;

        private bool dinkyEaten = false; //Boolean Value to track whether Dinky has been eaten or not, used to decide whether the Dinky animations will be normal or "frightened" from the Power Pellet
        private int dinkyEat = 0; //Integer value to track how many times the invincible player has touched Dinky. This is to make sure the player doesn't reap too many points from making contact
        //with Dinky, therefore only allowing points to be granted from the first touch.
        private bool ghostAbleToMove = true; //Boolean value to allow ghosts to move in certain situations (when the "Freeze Ghosts" support effect isn't in effect)

        public PacManLevel()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            playerBegin();
            button1.BringToFront();
            button2.BringToFront();
            Player.BringToFront();
            Player.Location = new Point(358, 590);
            PlayerHitbox.Location = Player.Location;
            DinkyHitbox.Location = Dinky.Location;
            ableToUp = true;
            ableToDown = true;
            ableToLeft = true;
            ableToRight = true;
            lastAction = "N/A";
            Player.BackColor = System.Drawing.Color.Transparent;

            button2.Enabled = false;
            button2.Visible = false;
            button1.Enabled = false;
            button1.Visible = false;

            Goldie.Visible = false;
            Blinky.Visible = false;

            label20.Visible = false;
            label21.Visible = false;
            label22.Visible = false;

            label12.Visible = false;
            label13.Visible = false;
            label14.Visible = false;
            label15.Visible = false;


            LevelCompleteText.Visible = false;
            GameOverText.Visible = false;

            BeginGame.Start();

            Dinky.Location = OuterPathTile1.Location;
            Dinky.Image = Properties.Resources.DinkyDown;

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "Fruit")
                {
                    control.Visible = false;
                    control.Left = control.Left + 700;
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "Intersection")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "Pathway")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "ThreewayUp")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "ThreewayDown")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "ThreewayLeft")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "ThreewayRight")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "Barrier")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "Node")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "UpTurn")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "DownTurn")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "LeftTurn")
                {
                    control.Visible = false;
                }
                if (control.Tag != null && control.Tag.ToString() == "RightTurn")
                {
                    control.Visible = false;
                }
            }
        }

        //Starting Position animation (depending on which character is being played)
        public void playerBegin()
        {
            if (Settings.setOption == 1)
            {
                Player.Image = Properties.Resources.Pac_Man_Eating_Left;
            }
            if (Settings.setOption == 2)
            {
                Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Left;
            }
            
        }

        //
        //Player Controls and Animations
        //

        //ControlTimerTick, Interval: 80, Used for most events in the game, such as controls, one ups, displaying effects, end of level, level bounds, etc.
        private void timer1_Tick(object sender, EventArgs e)
        {
            label6.Text = level.ToString();

            switch (supportEffect)
            {
                case 1:
                    label20.Text = "Freeze ghosts";
                    break;
                case 2:
                    label20.Text = "Invisibility";
                    break;
                case 3:
                    label20.Text = "Score Multiplier increased to x" + multiplier;
                    break;
                default:
                    label20.Text = "None";
                    break;
            }

            switch (dangerEffect)
            {
                case 1:
                    label21.Text = "Can't move";
                    break;
                case 2:
                    label21.Text = "Dinky doubled in size";
                    break;
                case 3:
                    label21.Text = "Score count stopped";
                    break;
                default:
                    label21.Text = "None";
                    break;
            }

            switch (randomEffect)
            {
                case 1:
                    label22.Text = "Freeze ghosts";
                    break;
                case 2:
                    label22.Text = "Invisibility";
                    break;
                case 3:
                    label22.Text = "Score Multiplier increased to x" + multiplier;
                    break;
                case 4:
                    label22.Text = "Can't move";
                    break;
                case 5:
                    label22.Text = "Dinky doubled in size";
                    break;
                case 6:
                    label22.Text = "Score count stopped";
                    break;
                default:
                    label22.Text = "None";
                    break;
            }

            PlayerHitbox.Location = Player.Location;
            DinkyHitbox.Location = Dinky.Location;

            PlayerControls();

            BarrierCollision();

            OneUpReset();

            if (itemsCollected == 242)
            {
                EndOfLevel.Start();
                LevelCompleteText.Visible = true;
                StopGame();
            }

            //Left Tunnel Bounds
            if (Player.Left == 46 && Player.Top == 386)
            {
                Player.Left = 658; //Teleports Pac-Man to Right Tunnel, 12 pixels to the left
            }
            //Right Tunnel Bounds
            if (Player.Left == 670 && Player.Top == 386)
            {
                Player.Left = 58; //Teleports Pac-Man to Left Tunnel, 12 pixels to the right
            }
        }

        private void OneUpReset()
        {
            if (oneUp >= 5000)
            {
                lives += 1;
                oneUp = 0;
            }
        }


        //PelletCollisionTimerTick, Interval: 100, For when a player collects a pellet
        private void PelletCollisionTimer_Tick(object sender, EventArgs e)
        {
            label7.Text = score.ToString();
            label18.Text = lives.ToString();
            label19.Text = oneUp.ToString();
            bool pelletCollect = false;

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "Pellet")
                {
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        pelletCollect = true;
                        control.Left = control.Left + 700;
                        if (pelletCollect == true)
                        {
                            itemsCollected += 1;
                            
                            if (ableToScore == true)
                            {
                                score = score + 10 * multiplier;
                                oneUp = oneUp + 10 * multiplier;
                            }
                        }
                    }
                }
            }
        }

        //Timer for when a player collects a fruit item
        private void FruitCollisionTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "Fruit")
                {
                    if (fruitAppear == 1)
                    {
                        if (nextFruit == 1)
                        {
                            Cherry.Visible = true;
                            Cherry.Location = new Point(364, 459);
                        }
                        if (nextFruit == 2)
                        {
                            Banana.Visible = true;
                            Banana.Location = new Point(364, 459);
                        }
                        if (nextFruit == 3)
                        {
                            Orange.Visible = true;
                            Orange.Location = new Point(364, 459);
                        }                        
                    }
                    
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        fruitAppear = 0;
                        control.Left = control.Left + 700;
                        if (control == Cherry)
                        {
                            if (ableToScore == true)
                            {
                                score = score + 300 * multiplier;
                                oneUp = oneUp + 300 * multiplier;
                            }
                        }
                        if (control == Banana)
                        {
                            if (ableToScore == true)
                            {
                                score = score + 750 * multiplier;
                                oneUp = oneUp + 750 * multiplier;
                            }
                        }
                        if (control == Orange)
                        {
                            if (ableToScore == true)
                            {
                                score = score + 500 * multiplier;
                                oneUp = oneUp + 500 * multiplier;
                            }
                        }
                    }
                }
            }
        }

        //Timer for when the player collects a special pellet (Power Pellet, Support Pellet, Danger Pellet, and Random Pellet
        private void SpecialPelletTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "PowerPellet")
                {
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        control.Left = control.Left + 700;
                        itemsCollected += 1;

                        if (ableToScore == true)
                        {
                            score = score + 50 * multiplier;
                            oneUp = oneUp + 50 * multiplier;
                        }
                        powerPelletCollected = powerPelletCollected + 1;
                    }
                }

                if (control.Tag != null && control.Tag.ToString() == "SupportPellet")
                {
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        supportEffectAvailable = true;
                        control.Left = control.Left + 700;
                        itemsCollected += 1;

                        if (ableToScore == true)
                        {
                            score = score + 25 * multiplier;
                            oneUp = oneUp + 25 * multiplier;                        
                        }
                        supportPelletCollected = supportPelletCollected + 1;
                    }
                }

                if (control.Tag != null && control.Tag.ToString() == "DangerPellet")
                {
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        dangerEffectAvailable = true;
                        control.Left = control.Left + 700;
                        itemsCollected += 1;

                        score = score + 100 * multiplier; 
                        oneUp = oneUp + 100 * multiplier;
                        dangerPelletCollected = dangerPelletCollected + 1;
                    }
                }


                if (control.Tag != null && control.Tag.ToString() == "RandomPellet")
                {
                    if (PlayerHitbox.Bounds.IntersectsWith(control.Bounds))
                    {
                        control.Visible = false;
                        randomEffectAvailable = true;
                        control.Left = control.Left + 700;
                        itemsCollected += 1;

                        if (ableToScore == true)
                        {
                            score = score + 50 * multiplier;
                            oneUp = oneUp + 50 * multiplier;
                        }
                        randomPelletCollected = randomPelletCollected + 1;
                    }
                }
            }

            PelletCollection();
        }
        
        //Procedure for when a special pellet is collected (enables timers and resets power timer whenever multipler power pellets are collected)
        public void PelletCollection()
        {
            if (powerPelletCollected == 1)
            {
                PowerPelletEffect();
                PowerPelletTimer.Enabled = true;
            }
            else if (powerPelletCollected >= 2)
            {
                powerTimerCount = 0;
                PowerPelletTimer.Enabled = true;
                powerPelletCollected = 1;
            }

            if (supportPelletCollected == 1 && supportEffectAvailable == true)
            {
                SupportPelletEffects();
                SupportPelletTimer.Enabled = true;
                label20.Visible = true;
            }
            else if (supportPelletCollected >= 2)
            {
                supportTimerCount = 0;
                SupportPelletTimer.Enabled = true;
                label20.Visible = true;
                supportPelletCollected = 1;
            }

            if (dangerPelletCollected == 1 && dangerEffectAvailable == true)
            {
                DangerPelletEffects();
                DangerPelletTimer.Enabled = true;
                label21.Visible = true;
            }
            else if (dangerPelletCollected >= 2)
            {
                dangerTimerCount = 0;
                DangerPelletTimer.Enabled = true;
                label21.Visible = true;
                dangerPelletCollected = 1;
            }

            if (randomPelletCollected == 1 && randomEffectAvailable == true)
            {
                RandomPelletEffects();
                RandomPelletTimer.Enabled = true;
                label22.Visible = true;
            }
            else if (randomPelletCollected >= 2)
            {
                randomTimerCount = 0;
                RandomPelletTimer.Enabled = true;
                label22.Visible = true;
                randomPelletCollected = 1;
            }
        }

        //Power Pellet invincibility (eating ghosts)
        private void PowerPelletEffect()
        {
            invincible = true;
        }

        //3 Support Pellet effects
        private void SupportPelletEffects()
        {
            Random rnd = new Random();
            supportEffect = rnd.Next(1, 4); //Random choice of effect
            supportEffectAvailable = false;

            if (supportEffect == 1) //ghosts freeze for 10 seconds
            {
                SupportEffect1();
                supportEffectDuration = 10;
            }
            if (supportEffect == 2) //invisibility
            {
                SupportEffect2();
                supportEffectDuration = 10;
            }
            if (supportEffect == 3) //changes score multiplier
            {
                SupportEffect3();
                supportEffectDuration = 10;
            }
        }

        public void SupportEffect1()
        {
            ghostAbleToMove = false;

            dinkyLeft = false;
            dinkyRight = false;
            dinkyUp = false;
            dinkyDown = false;
        }

        private void RemoveSupportEffect1()
        {
            ghostAbleToMove = true;

            if (lastDinkyAction == "Up")
            {
                dinkyLeft = false;
                dinkyRight = false;
                dinkyUp = true;
                dinkyDown = false;
            }
            if (lastDinkyAction == "Down")
            {
                dinkyLeft = false;
                dinkyRight = false;
                dinkyUp = false;
                dinkyDown = true;
            }
            if (lastDinkyAction == "Left")
            {
                dinkyLeft = true;
                dinkyRight = false;
                dinkyUp = false;
                dinkyDown = false;
            }
            if (lastDinkyAction == "Right")
            {
                dinkyLeft = false;
                dinkyRight = true;
                dinkyUp = false;
                dinkyDown = false;
            }
        }

        public void SupportEffect2()
        {
            invisible = true;
        }

        private void RemoveSupportEffect2()
        {
            invisible = false;
        }


        public void SupportEffect3()
        {
            Random multirnd = new Random();
            multiplier = multirnd.Next(2, 5); //Random choice of multiplier
            supportEffectDuration = 10;
        }

        private void RemoveSupportEffect3()
        {
            multiplier = 1;
        }

        //3 Danger Pellet effects
        private void DangerPelletEffects()
        {
            Random rnd = new Random();
            dangerEffect = rnd.Next(1, 4); //Random choice of effect
            dangerEffectAvailable = false;

            if (dangerEffect == 1) //player freezes for 2 seconds
            {
                DangerEffect1();
                dangerEffectDuration = 10;
            }
            if (dangerEffect == 2) //random ghost becomes bigger (only Dinky in this case)
            {
                DangerEffect2();
                dangerEffectDuration = 5;
            }
            if (dangerEffect == 3) //no more points gained (for 10 seconds)
            {
                DangerEffect3();
                dangerEffectDuration = 10;
            }
        }

        public void DangerEffect1()
        {
            ableToMove = false;
        }

        private void RemoveDangerEffect1()
        {
            ableToMove = true;
        }

        public void DangerEffect2()
        {
            Dinky.Size = new Size(106, 92);
        }

        private void RemoveDangerEffect2()
        {
            Dinky.Size = new Size(40, 38);
        }

        public void DangerEffect3()
        {
            ableToScore = false;
        }

        private void RemoveDangerEffect3()
        {
            ableToScore = true;
        }

        //6 Random Pellet effects (uses Support and Danger effects)
        private void RandomPelletEffects()
        {
            Random rnd = new Random();
            randomEffect = rnd.Next(1, 7); //Random choice of effect
            randomEffectAvailable = false;

            if (randomEffect == 1)
            {
                SupportEffect1();
                randomEffectDuration = 10;
            }
            if (randomEffect == 2)
            {
                SupportEffect2();
                randomEffectDuration = 10;
            }
            if (randomEffect == 3)
            {
                SupportEffect3();
                randomEffectDuration = 10;
            }
            if (randomEffect == 4)
            {
                DangerEffect1();
                randomEffectDuration = 10;
            }
            if (randomEffect == 5)
            {
                DangerEffect2();
                randomEffectDuration = 5;
            }
            if (randomEffect == 6)
            {
                DangerEffect3();
                randomEffectDuration = 10;
            }
        }

        //Timer for what happens during the time a Power Pellet is active
        private void PowerPelletTimer_Tick(object sender, EventArgs e)
        {
            powerTimerCount++;
            label12.Visible = true;
            label12.Text = powerTimerCount.ToString();

            if (powerTimerCount == 10)
            {
                label12.Visible = false;
                powerPelletCollected = 0;
                invincible = false;
                dinkyEaten = false;
                dinkyEat = 0;
                setFruit();
                fruitAppear = fruitAppear + 1;
                PowerPelletTimer.Enabled = false;
                powerTimerCount = 0;
            }
        }

        private void setFruit()
        {
            Random rnd = new Random();
            nextFruit = rnd.Next(1, 4);
        }

        //Timer for what happens during the time a Support Pellet is active
        private void SupportPelletTimer_Tick(object sender, EventArgs e)
        {
            supportTimerCount++;
            label13.Visible = true;
            label13.Text = supportTimerCount.ToString();

            if (supportTimerCount == 2)
            {
                label20.Visible = false;
            }

            if (supportTimerCount == supportEffectDuration)
            {
                label13.Visible = false;
                label20.Visible = false;
                supportPelletCollected = 0;
                supportEffectDuration = 0;
                if (supportEffect == 1)
                {
                    //remove effect 1
                    RemoveSupportEffect1();
                }
                if (supportEffect == 2)
                {
                    //remove effect 2
                    RemoveSupportEffect2();
                }
                if (supportEffect == 3)
                {
                    //remove effect 3
                    RemoveSupportEffect3();
                }

                supportEffect = 0;
                SupportPelletTimer.Enabled = false;
                supportTimerCount = 0;
            }
        }

        //Timer for what happens during the time a Danger Pellet is active
        private void DangerPelletTimer_Tick(object sender, EventArgs e)
        {
            dangerTimerCount++;
            label14.Visible = true;
            label14.Text = dangerTimerCount.ToString();

            if (dangerTimerCount == 2)
            {
                label21.Visible = false;
            }
            
            if (dangerTimerCount == dangerEffectDuration)
            {
                label14.Visible = false;
                dangerPelletCollected = 0;
                dangerEffectDuration = 0;
                if (dangerEffect == 1)
                {
                    //remove effect 1
                    RemoveDangerEffect1();
                }
                if (dangerEffect == 2)
                {
                    //remove effect 2
                    RemoveDangerEffect2();
                }
                if (dangerEffect == 3)
                {
                    //remove effect 3
                    RemoveDangerEffect3();
                }

                dangerEffect = 0;
                DangerPelletTimer.Enabled = false;
                dangerTimerCount = 0;
            }
        }

        //Timer for what happens during the time a Random Pellet is active
        private void RandomPelletTimer_Tick(object sender, EventArgs e)
        {
            randomTimerCount++;
            label15.Visible = true;
            label15.Text = randomTimerCount.ToString();

            if (randomTimerCount == 2)
            {
                label22.Visible = false;
            }

            if (randomTimerCount == randomEffectDuration)
            {
                label15.Visible = false;
                randomPelletCollected = 0;
                randomEffectDuration = 0;
                if (randomEffect == 1)
                {
                    RemoveSupportEffect1();
                }
                if (randomEffect == 2)
                {
                    RemoveSupportEffect2();
                }
                if (randomEffect == 3)
                {
                    RemoveSupportEffect3();
                }
                if (randomEffect == 4)
                {
                    RemoveDangerEffect1();
                }
                if (randomEffect == 5)
                {
                    RemoveDangerEffect2();
                }
                if (randomEffect == 6)
                {
                    RemoveDangerEffect3();
                }
                randomEffect = 0;
                RandomPelletTimer.Enabled = false;
                randomTimerCount = 0;
            }
        }

        //Prevents barriers from being crossed (this was used earlier in coding, when pathways and intersections weren't established yet, the barriers were mainly used to pave where
        //the pathways and intersections were going to be)
        public void BarrierCollision()
        {
            foreach (Control barrier in this.Controls)
            {
                if (barrier.Tag != null && barrier.Tag.ToString() == "Barrier")
                {
                    if (Player.Bounds.IntersectsWith(barrier.Bounds) && moveLeft == true)
                    {
                        Player.Left = Player.Left + speed;
                        moveLeft = false;
                    }

                    if (Player.Bounds.IntersectsWith(barrier.Bounds) && moveRight == true)
                    {
                        Player.Left = Player.Left - speed;
                        moveRight = false;
                    }

                    if (Player.Bounds.IntersectsWith(barrier.Bounds) && moveUp == true)
                    {
                        Player.Top = Player.Top + speed;
                        moveUp = false;
                    }

                    if (Player.Bounds.IntersectsWith(barrier.Bounds) && moveDown == true)
                    {
                        Player.Top = Player.Top - speed;
                        moveDown = false;
                    }
                }
            }
        }

        //Procedure where the player moves based on controls
        private void PlayerControls()
        {
            if (moveUp == true && Player.Top > -60)
            {
                Player.Top = Player.Top - speed;
            }
            if (moveDown == true && Player.Top < 842)
            {
                Player.Top = Player.Top + speed;
            }
            if (moveLeft == true && Player.Left > -60)
            {
                Player.Left = Player.Left - speed;
            }
            if (moveRight == true && Player.Left < 699)
            {
                Player.Left = Player.Left + speed;
            }
        }

        //Timer for the behaviour of pathways, where the player can only move horizontally, if they've entered a pathway horizontally, or vertically, if they've entered a pathway vertically
        private void PathwayTimer_Tick(object sender, EventArgs e)
        {
            bool pathwayTouched = false;

            foreach (Control path in this.Controls)
            {
                if (path.Tag != null && path.Tag.ToString() == "Pathway")
                {
                    if (Player.Bounds.IntersectsWith(path.Bounds))
                    {
                        pathwayTouched = true;
                        veryLastAction = lastAction;
                    }
                }
            }

            if (pathwayTouched == true)
            {
                if (veryLastAction == "Up")
                {
                    ableToLeft = false;
                    ableToRight = false;
                    ableToUp = true;
                    ableToDown = true;
                }
                if (veryLastAction == "Down")
                {
                    ableToLeft = false;
                    ableToRight = false;
                    ableToUp = true;
                    ableToDown = true;
                }
                if (veryLastAction == "Left")
                {
                    ableToLeft = true;
                    ableToRight = true;
                    ableToUp = false;
                    ableToDown = false;
                }
                if (veryLastAction == "Right")
                {
                    ableToLeft = true;
                    ableToRight = true;
                    ableToUp = false;
                    ableToDown = false;
                }
                else if (veryLastAction == "N/A")
                {
                    ableToLeft = true;
                    ableToRight = true;
                    ableToUp = false;
                    ableToDown = false;
                }
            }
            else if (pathwayTouched == false)
            {
                ableToLeft = true;
                ableToRight = true;
                ableToUp = true;
                ableToDown = true;
            }
        }

        //PlayerHitboxTimerTick, Interval: 1, Constantly places Hitbox at Player's location
        private void PlayerHitboxTimer_Tick(object sender, EventArgs e)
        {
            PlayerHitbox.Location = Player.Location;
        }

        //DinkyHitboxTimerTick, Interval 1, Constantly places Hitbox at Dinky's location
        private void DinkyHitboxTimer_Tick(object sender, EventArgs e)
        {
            DinkyHitbox.Location = Dinky.Location;
        }

        //When the user presses WASD or Arrow Keys
        private void PacManLevel_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                if (ableToMove == true)
                {
                    PushMove(moveList, "Up");
                    if (moveList.Count == 2)
                    {
                        RemoveMove(moveList, 0);
                    }

                    PushMove(actionList, "Up");
                    if (actionList.Count == 3)
                    {
                        RemoveMove(actionList, 0);
                    }
                }

                if (ableToUp == true && ableToMove == true)
                {
                    lastAction = "Up";

                    moveUp = true;
                    moveDown = false;
                    moveLeft = false;
                    moveRight = false;

                    upButtonPress = true;
                    downButtonPress = false;
                    leftButtonPress = false;
                    rightButtonPress = false;

                }
            }

            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                if (ableToMove == true)
                {
                    PushMove(moveList, "Down");
                    if (moveList.Count == 2)
                    {
                        RemoveMove(moveList, 0);
                    }

                    PushMove(actionList, "Down");
                    if (actionList.Count == 3)
                    {
                        RemoveMove(actionList, 0);
                    }
                }

                if (ableToDown == true && ableToMove == true)
                {
                    lastAction = "Down";

                    moveUp = false;
                    moveDown = true;
                    moveLeft = false;
                    moveRight = false;

                    upButtonPress = false;
                    downButtonPress = true;
                    leftButtonPress = false;
                    rightButtonPress = false;

                }
            }

            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                if (ableToMove == true)
                {
                    PushMove(moveList, "Left");
                    if (moveList.Count == 2)
                    {
                        RemoveMove(moveList, 0);
                    }

                    PushMove(actionList, "Left");
                    if (actionList.Count == 3)
                    {
                        RemoveMove(actionList, 0);
                    }
                }

                if (ableToLeft == true && ableToMove == true)
                {
                    lastAction = "Left";

                    moveUp = false;
                    moveDown = false;
                    moveLeft = true;
                    moveRight = false;

                    upButtonPress = false;
                    downButtonPress = false;
                    leftButtonPress = true;
                    rightButtonPress = false;

                }
            }

            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                if (ableToMove == true)
                {
                    PushMove(moveList, "Right");
                    if (moveList.Count == 2)
                    {
                        RemoveMove(moveList, 0);
                    }

                    PushMove(actionList, "Right");
                    if (actionList.Count == 3)
                    {
                        RemoveMove(actionList, 0);
                    }
                }

                if (ableToRight == true && ableToMove == true)
                {
                    lastAction = "Right";

                    moveUp = false;
                    moveDown = false;
                    moveLeft = false;
                    moveRight = true;

                    upButtonPress = false;
                    downButtonPress = false;
                    leftButtonPress = false;
                    rightButtonPress = true;

                }
            }

            //Pause Key
            if (e.KeyCode == Keys.P && ableToPause == true)
            {
                button1.BringToFront();
                button2.BringToFront();

                button2.Enabled = true;
                button2.Visible = true;
                button1.Enabled = true;
                button1.Visible = true;

                StopGame();

                if (PowerPelletTimer.Enabled == true)
                {
                    PowerPelletTimer.Enabled = false;
                    PowerPelletTimerPaused = true;
                }
                if (SupportPelletTimer.Enabled == true)
                {
                    SupportPelletTimer.Enabled = false;
                    SupportPelletTimerPaused = true;
                }
                if (DangerPelletTimer.Enabled == true)
                {
                    DangerPelletTimer.Enabled = false;
                    DangerPelletTimerPaused = true;
                }
                if (RandomPelletTimer.Enabled == true)
                {
                    RandomPelletTimer.Enabled = false;
                    RandomPelletTimerPaused = true;
                }
            }
        }

        //The timers that are enabled when the game starts or resumes
        private void StartGame()
        {
            ControlTimer.Enabled = true;
            PelletCollisionTimer.Enabled = true;
            FruitCollisionTimer.Enabled = true;
            PlayerHitboxTimer.Enabled = true;
            DinkyHitboxTimer.Enabled = true;
            SpecialPelletTimer.Enabled = true;
            PathwayTimer.Enabled = true;
            IntersectionTimer.Enabled = true;
            AnimationTimer.Enabled = true;
            ThreewayTimer.Enabled = true;
            DinkyBlueGhostTimer.Enabled = true;
            DinkyPathTimer.Enabled = true;
        }

        //The timers that are disabled when the game pauses or ends
        private void StopGame()
        {
            ControlTimer.Enabled = false;
            PelletCollisionTimer.Enabled = false;
            FruitCollisionTimer.Enabled = false;
            PlayerHitboxTimer.Enabled = false;
            DinkyHitboxTimer.Enabled = false;
            SpecialPelletTimer.Enabled = false;
            DinkyBlueGhostTimer.Enabled = false;
            DinkyPathTimer.Enabled = false;
            AnimationTimer.Enabled = false;
        }

        //The events that occur when the game restarts after a life loss or a level progression
        private void RestartGame()
        {
            StartGame();
            Player.Location = new Point(358, 590);
            PlayerHitbox.Location = Player.Location;
            Dinky.Location = OuterPathTile1.Location;
            DinkyHitbox.Location = Dinky.Location;
            moveUp = false;
            moveDown = false;
            moveLeft = false;
            moveRight = false;
            lastAction = "Left"; //To prevent player from being stuck in starting pathway
        }

        private void PacManAnimations()
        {
            PacManLeftAnimation();
            PacManRightAnimation();
            PacManUpAnimation();
            PacManDownAnimation();
        }

        private void MsPacManAnimations()
        {
            MsPacManLeftAnimation();
            MsPacManRightAnimation();
            MsPacManUpAnimation();
            MsPacManDownAnimation();
        }

        //Pac-Man Animations for both Normal and Invisible modes
        private void PacManLeftAnimation()
        {
            if (leftButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Pac_Man_Eating_Left;
            }
            else if (leftButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Pac_Man_Left_Animation__invisible_;
            }
        }

        private void PacManRightAnimation()
        {
            if (rightButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Pac_Man_Eating_Right;
            }
            else if (rightButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Pac_Man_Right_Animation__invisible_;
            }
        }

        private void PacManUpAnimation()
        {
            if (upButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Pac_Man_Eating_Up;
            }
            else if (upButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Pac_Man_Up_Animation__invisible_;
            }
        }

        private void PacManDownAnimation()
        {
            if (downButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Pac_Man_Eating_Down;
            }
            else if (downButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Pac_Man_Down_Animation__invisible_;
            }
        }

        //Ms. Pac-Man Animations for both Normal and Invisible modes
        private void MsPacManLeftAnimation()
        {
            if (leftButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Left;
            }
            else if (leftButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Left__invisible_;
            }
        }

        private void MsPacManRightAnimation()
        {
            if (rightButtonPress == true && invisible == false)
            {
                Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Right;
            }
            else if (rightButtonPress == true && invisible == true)
            {
                Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Right__invisible_;
            }
        }


        private void MsPacManUpAnimation()
        {
            if (upButtonPress == true && invisible == false)
            {
                if (actionList[1] == "Left") //Work in Progress code for making Ms. Pac-Man's animations more natural when the player goes Up or Down (unfinished for NEA project)
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up;
                }
                if (actionList[1] == "Right")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up_2;
                }
                if (actionList[1] == "Up")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up;
                }
                if (actionList[1] == "Down")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up_2;
                }
            }
            else if (upButtonPress == true && invisible == true)
            {
                if (actionList[1] == "Left")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up__invisible_;
                }
                if (actionList[1] == "Right")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up_2__invisible_;
                }
                if (actionList[1] == "Up")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up__invisible_;
                }
                if (actionList[1] == "Down")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Up_2__invisible_;
                }
            }
        }

        private void MsPacManDownAnimation()
        {
            if (downButtonPress == true && invisible == false)
            {
                if (actionList[1] == "Left") //Work in Progress code for making Ms. Pac-Man's animations more natural when the player goes Up or Down (unfinished for NEA project)
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down;
                }
                if (actionList[1] == "Right")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down_2; 
                }
                if (actionList[1] == "Up")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down;
                }
                if (actionList[1] == "Down")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down_2;
                }
            }
            else if (downButtonPress == true && invisible == true)
            {
                if (actionList[1] == "Left")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down__invisible_;
                }
                if (actionList[1] == "Right")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down_2__invisible_;
                }
                if (actionList[1] == "Up")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down__invisible_;
                }
                if (actionList[1] == "Down")
                {
                    Player.Image = Properties.Resources.Ms__Pac_Man_Eating_Down_2__invisible_;
                }
            }
        }
        
        //Quit Game Button
        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            MainMenu m = new MainMenu();
            m.Show();
        }

        //Resume Button
        private void button2_Click(object sender, EventArgs e)
        {
            StartGame();

            if (PowerPelletTimerPaused == true)
            {
                PowerPelletTimer.Enabled = true;
                PowerPelletTimerPaused = false;
            }
            if (SupportPelletTimerPaused == true)
            {
                SupportPelletTimer.Enabled = true;
                SupportPelletTimerPaused = false;
            }
            if (DangerPelletTimerPaused == true)
            {
                DangerPelletTimer.Enabled = true;
                DangerPelletTimerPaused = false;
            }
            if (RandomPelletTimerPaused == true)
            {
                RandomPelletTimer.Enabled = true;
                RandomPelletTimerPaused = false;
            }

            button1.SendToBack();
            button2.SendToBack();

            button2.Enabled = false;
            button1.Enabled = false;
            
        }

        //Designed so if the player was travelling horizontally and reaches an intersection, they can start travelling vertically at that intersection, and vice versa for travelling vertically
        private void IntersectionTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control intersection in this.Controls)
            {
                if (intersection.Tag != null && intersection.Tag.ToString() == "Intersection")
                {
                    if (Player.Location == intersection.Location)
                    {
                        if (moveList[0] == "Up")
                        {
                            ableToUp = true;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = false;
                            
                            lastAction = "Up";
                            moveUp = true;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = true;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Down")
                        {
                            ableToUp = false;
                            ableToDown = true;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Down";
                            moveUp = false;
                            moveDown = true;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = true;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Left")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = true;
                            ableToRight = false;

                            lastAction = "Left";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = true;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = true;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Right")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = true;

                            lastAction = "Right";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = true;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = true;

                        }

                    }
                }
            }
        }

        //Used for the moveLists, actionLists
        private void PushMove(List<string> list, string move)
        {
            list.Add(move);
        }

        //Used for the moveLists, actionLists
        public void RemoveMove(List<string> list, int itemAtPosition)
        {
            list.RemoveAt(itemAtPosition);
        }

        //Changes player animation depending on the customisation option they chose if they went into the settings
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (Settings.setOption == 1)
            {
                PacManAnimations();
            }
            if (Settings.setOption == 2)
            {
                MsPacManAnimations();
            }
        }

        //For the special intersections, where you can only go 3 directions rather than 4. This makes controls smoother
        private void ThreewayTimer_Tick(object sender, EventArgs e)
        {
            foreach(Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "ThreewayUp")
                {
                    if (Player.Location == control.Location)
                    {
                        if (moveList[0] == "Up")
                        {
                            ableToUp = true;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Up";
                            moveUp = true;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = true;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Left")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = true;
                            ableToRight = false;

                            lastAction = "Left";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = true;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = true;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Right")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = true;

                            lastAction = "Right";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = true;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = true;

                        }
                    }
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "ThreewayDown")
                {
                    if (Player.Location == control.Location)
                    {
                        if (moveList[0] == "Down")
                        {
                            ableToUp = false;
                            ableToDown = true;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Down";
                            moveUp = false;
                            moveDown = true;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = true;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Left")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = true;
                            ableToRight = false;

                            lastAction = "Left";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = true;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = true;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Right")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = true;

                            lastAction = "Right";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = true;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = true;

                        }
                    }
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "ThreewayLeft")
                {
                    if (Player.Location == control.Location)
                    {
                        if (moveList[0] == "Up")
                        {
                            ableToUp = true;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Up";
                            moveUp = true;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = true;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Down")
                        {
                            ableToUp = false;
                            ableToDown = true;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Down";
                            moveUp = false;
                            moveDown = true;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = true;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Left")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = true;
                            ableToRight = false;

                            lastAction = "Left";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = true;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = true;
                            rightButtonPress = false;

                        }
                    }
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "ThreewayRight")
                {
                    if (Player.Location == control.Location)
                    {
                        if (moveList[0] == "Up")
                        {
                            ableToUp = true;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Up";
                            moveUp = true;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = true;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Down")
                        {
                            ableToUp = false;
                            ableToDown = true;
                            ableToLeft = false;
                            ableToRight = false;

                            lastAction = "Down";
                            moveUp = false;
                            moveDown = true;
                            moveLeft = false;
                            moveRight = false;

                            upButtonPress = false;
                            downButtonPress = true;
                            leftButtonPress = false;
                            rightButtonPress = false;

                        }
                        if (moveList[0] == "Right")
                        {
                            ableToUp = false;
                            ableToDown = false;
                            ableToLeft = false;
                            ableToRight = true;

                            lastAction = "Right";
                            moveUp = false;
                            moveDown = false;
                            moveLeft = false;
                            moveRight = true;

                            upButtonPress = false;
                            downButtonPress = false;
                            leftButtonPress = false;
                            rightButtonPress = true;

                        }
                    }
                }
            }
        }

        private void InkyCyanGhostTimer_Tick(object sender, EventArgs e)
        {
            //timer for Inky Ghost
        }

        private void BlinkyRedGhostTimer_Tick(object sender, EventArgs e)
        {
            //timer for Blinky Ghost
        }

        private void PinkyPinkGhostTimer_Tick(object sender, EventArgs e)
        {
            //timer for Pinky Ghost
        }

        private void ClydeOrangeGhostTimer_Tick(object sender, EventArgs e)
        {
            //timer for Clyde Ghost
        }

        private void GoldieGoldGhostTimer_Tick(object sender, EventArgs e)
        {
            //timer for Goldie Ghost
        }

        //Timer for Dinky Ghost, consisting of movement, eating the player, and sprites
        private void DinkyBlueGhostTimer_Tick(object sender, EventArgs e)
        {
            if (dinkyUp == true)
            {
                Dinky.Top = Dinky.Top - speed;
            }
            if (dinkyDown == true)
            {
                Dinky.Top = Dinky.Top + speed;
            }
            if (dinkyLeft == true)
            {
                Dinky.Left = Dinky.Left - speed;
            }
            if (dinkyRight == true)
            {
                Dinky.Left = Dinky.Left + speed;
            }

            if (Dinky.Bounds.IntersectsWith(PlayerHitbox.Bounds))
            {
                if (invincible == true)
                {
                    dinkyEaten = true;
                    dinkyEat += 1;
                    if (dinkyEat <= 1)
                    {
                        score += 200;
                    }
                }
                else if (invincible == false && invisible == false)
                {
                    StopGame();
                    ableToPause = false;
                    if (lives == 0)
                    {
                        DeathTimer.Start();
                        Player.Image = Properties.Resources.Pac_Man_Death_Animation__extended_;
                    }
                    else
                    {
                        ResetTimer.Start();
                        Player.Image = Properties.Resources.Pac_Man_Death_Animation__extended_;

                    }
                }
            }
            else
            {
                ableToPause = true;
            }

            DinkySprite();

        }

        //When Dinky turns left
        private void DinkyLeftAnimation()
        {
            if (dinkyLeft == true && invincible == false)
            {
                Dinky.Image = Properties.Resources.DinkyLeft;
            }
            else if (dinkyLeft == true && invincible == true && dinkyEaten == true && dinkyEat >= 1)
            {
                Dinky.Image = Properties.Resources.DinkyEatenLeft;
            }
            else if (invincible == true && dinkyEaten == false)
            {
                Dinky.Image = Properties.Resources.Vulnerable_ghost;
            }
        }

        //When Dinky turns right
        private void DinkyRightAnimation()
        {
            if (dinkyRight == true && invincible == false)
            {
                Dinky.Image = Properties.Resources.DinkyRight;
            }
            else if (dinkyRight == true && invincible == true && dinkyEaten == true && dinkyEat >= 1)
            {
                Dinky.Image = Properties.Resources.DinkyEatenRight;
            }
            else if (invincible == true && dinkyEaten == false)
            {
                Dinky.Image = Properties.Resources.Vulnerable_ghost;
            }
        }

        //When Dinky goes up
        private void DinkyUpAnimation()
        {
            if (dinkyUp == true && invincible == false)
            {
                Dinky.Image = Properties.Resources.DinkyUp;
            }
            else if (dinkyUp == true && invincible == true && dinkyEaten == true && dinkyEat >= 1)
            {
                Dinky.Image = Properties.Resources.DinkyEatenUp;
            }
            else if (invincible == true && dinkyEaten == false)
            {
                Dinky.Image = Properties.Resources.Vulnerable_ghost;
            }
        }

        //When Dinky goes down
        private void DinkyDownAnimation()
        {
            if (dinkyDown == true && invincible == false)
            {
                Dinky.Image = Properties.Resources.DinkyDown;
            }
            else if (dinkyDown == true && invincible == true && dinkyEaten == true && dinkyEat >= 1)
            {
                Dinky.Image = Properties.Resources.DinkyEatenDown;
            }
            else if (invincible == true && dinkyEaten == false)
            {
                Dinky.Image = Properties.Resources.Vulnerable_ghost;
            }
        }


        private void DinkySprite()
        {
            DinkyLeftAnimation();
            DinkyRightAnimation();
            DinkyUpAnimation();
            DinkyDownAnimation();
        }

        //Timer for Dinky's path around the maze
        private void DinkyPathTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control box in this.Controls)
            {
                if (DinkyHitbox.Location == box.Location && box.Tag.ToString() == "DownTurn" && ghostAbleToMove == true)
                {
                    dinkyUp = false;
                    dinkyDown = true;
                    dinkyLeft = false;
                    dinkyRight = false;

                    lastDinkyAction = "Down";
                }
                if (DinkyHitbox.Location == box.Location && box.Tag.ToString() == "RightTurn" && ghostAbleToMove == true)
                {
                    dinkyUp = false;
                    dinkyDown = false;
                    dinkyLeft = false;
                    dinkyRight = true;

                    lastDinkyAction = "Right";
                }
                if (DinkyHitbox.Location == box.Location && box.Tag.ToString() == "UpTurn" && ghostAbleToMove == true)
                {
                    dinkyUp = true;
                    dinkyDown = false;
                    dinkyLeft = false;
                    dinkyRight = false;

                    lastDinkyAction = "Up";
                }
                if (DinkyHitbox.Location == box.Location && box.Tag.ToString() == "LeftTurn" && ghostAbleToMove == true)
                {
                    dinkyUp = false;
                    dinkyDown = false;
                    dinkyLeft = true;
                    dinkyRight = false;

                    lastDinkyAction = "Left";
                }
            }
        }


        //Timer for when the player loses all their lives (lasting 3 seconds)
        private void DeathTimer_Tick(object sender, EventArgs e)
        {
            deathTimerCount++;
            GameOverText.Visible = true;

            if (deathTimerCount == 3)
            {
                this.Visible = false;
                MainMenu m = new MainMenu();
                m.Show();
                DeathTimer.Stop();

                GameOverText.Visible = false;
            }
        }

        //Timer for when the player loses a life but has at least one life left (lasting 3 seconds)
        private void ResetTimer_Tick(object sender, EventArgs e)
        {
            resetTimerCount++;

            if (resetTimerCount == 3)
            {
                lives -= 1;
                RestartGame();
                resetTimerCount = 0;
                ResetTimer.Stop();
            }
        }

        //Procedure for when the player completes a level (all the items are placed back in their positions)
        private void LevelComplete()
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "Pellet")
                {
                    control.Left = control.Left - 700;
                    control.Visible = true;
                }
                if (control.Tag != null && control.Tag.ToString() == "PowerPellet")
                {
                    control.Left = control.Left - 700;
                    control.Visible = true;
                }
                if (control.Tag != null && control.Tag.ToString() == "SupportPellet")
                {
                    control.Left = control.Left - 700;
                    control.Visible = true;
                }
                if (control.Tag != null && control.Tag.ToString() == "DangerPellet")
                {
                    control.Left = control.Left - 700;
                    control.Visible = true;
                }
                if (control.Tag != null && control.Tag.ToString() == "RandomPellet")
                {
                    control.Left = control.Left - 700;
                    control.Visible = true;
                }
            }
        }

        //Timer for when the game/first level begins (lasts 5 seconds)
        private void BeginGame_Tick(object sender, EventArgs e)
        {
            startCount++;
            
            Ready.Visible = true;

            if (startCount == 5)
            {
                Ready.Visible = false;
                StartGame();
                ableToPause = true;
                BeginGame.Stop();
                startCount = 0;
                ableToMove = true;
            }
        }

        //Timer for when a level ends (when all of the pellets are picked up, lasts 5 seconds) 
        private void EndOfLevel_Tick(object sender, EventArgs e)
        {
            endCount++;

            if (endCount == 5)
            {
                LevelCompleteText.Visible = false;
                itemsCollected = 0;
                level += 1;
                LevelComplete();
                veryLastAction = "Left"; //To prevent player from being stuck in starting pathway (if their very last action was Up or Down
                RestartGame();
                EndOfLevel.Stop();
                endCount = 0;
            }
        }
    }
}