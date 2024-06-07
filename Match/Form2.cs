using Match;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Match3
{
    public partial class Form2 : Form
    {
        private Button[,] buttonsGrid;
        private const int gridSize = 8;
        private Dictionary<int, string> symbols = new Dictionary<int, string> {
            { 1, "♦" },
            { 2, "♣" },
            { 3, "♥" },
            { 4, "♠" },
            { 5, "♤" }
        };
        private Button selectedButton;
        private Button otherButton;

        private int score;
        private Label scoreLabel;

        private Timer timer;
        private Label timeLabel;
        private int remainingTime = 60;

        public Form2()
        {
            InitializeComponent();
            InitializeGame();
            CenterToScreen();

            score = 0;
            scoreLabel = new Label();
            scoreLabel.Text = "Score: " + score;
            scoreLabel.Location = new Point(10, 10);
            this.Controls.Add(scoreLabel);

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();

            timeLabel = new Label();
            timeLabel.Text = "Time: " + remainingTime + " s.";
            timeLabel.Location = new Point(this.ClientSize.Width - timeLabel.Size.Width - 10, 10);
            timeLabel.Size = new Size(55, 55);
            this.Controls.Add(timeLabel);
            timeLabel.Text = remainingTime.ToString();
        }

        private void InitializeGame()
        {
            buttonsGrid = new Button[gridSize, gridSize];
            Random random = new Random();

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Button button = new Button();
                    button.Text = symbols[random.Next(1, 6)];
                    button.Size = new Size(45, 45);
                    button.Location = new Point((this.ClientSize.Width - gridSize * button.Width) / 2 + i * button.Width, (this.ClientSize.Height - gridSize * button.Height) / 2 + j * button.Height);
                    button.Tag = random.Next(1, 6);
                    button.Click += Button_Click;
                    buttonsGrid[i, j] = button;
                    this.Controls.Add(button);
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            if (selectedButton == null)
            {
                selectedButton = clickedButton;
            }
            else
            {
                otherButton = clickedButton;

                if (AreNeighbors(selectedButton, otherButton))
                {
                    SwapSymbols(selectedButton, otherButton);


                    CheckAndRemoveMatches();

                    selectedButton = null;
                    otherButton = null;
                }
            }
        }

        private void SwapSymbols(Button button1, Button button2)
        {
            int symbol1 = (int)button1.Tag;
            int symbol2 = (int)button2.Tag;

            button1.Tag = symbol2;
            button1.Text = symbols[symbol2];

            button2.Tag = symbol1;
            button2.Text = symbols[symbol1];
        }

        private bool AreNeighbors(Button button1, Button button2)
        {
            int button1Row = -1;
            int button1Col = -1;
            int button2Row = -1;
            int button2Col = -1;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (buttonsGrid[i, j] == button1)
                    {
                        button1Row = i;
                        button1Col = j;
                    }
                    if (buttonsGrid[i, j] == button2)
                    {
                        button2Row = i;
                        button2Col = j;
                    }
                }
            }

            return (Math.Abs(button1Row - button2Row) == 1 && button1Col == button2Col) ||
                   (Math.Abs(button1Col - button2Col) == 1 && button1Row == button2Row);
        }

        private bool HasMatch(Button button)
        {
            int buttonRow = -1;
            int buttonCol = -1;

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (buttonsGrid[i, j] == button)
                    {
                        buttonRow = i;
                        buttonCol = j;
                        break;
                    }
                }
                if (buttonRow != -1 && buttonCol != -1)
                {
                    break;
                }
            }

            int symbol = (int)button.Tag;

            // Horizontal check
            int count = 1;
            int tempCol = buttonCol - 1;
            while (tempCol >= 0 && (int)buttonsGrid[buttonRow, tempCol].Tag == symbol)
            {
                count++;
                tempCol--;
            }
            tempCol = buttonCol + 1;
            while (tempCol < gridSize && (int)buttonsGrid[buttonRow, tempCol].Tag == symbol)
            {
                count++;
                tempCol++;
            }
            if (count >= 3)
            {
                return true;
            }

            // Vertical check
            count = 1;
            int tempRow = buttonRow - 1;
            while (tempRow >= 0 && (int)buttonsGrid[tempRow, buttonCol].Tag == symbol)
            {
                count++;
                tempRow--;
            }
            tempRow = buttonRow + 1;
            while (tempRow < gridSize && (int)buttonsGrid[tempRow, buttonCol].Tag == symbol)
            {
                count++;
                tempRow++;
            }
            if (count >= 3)
            {
                return true;
            }

            return false;
        }

        private void CheckAndRemoveMatches()
        {
            List<Button> buttonsToRemove = new List<Button>();

            for (int i = gridSize - 1; i >= 0; i--)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (HasMatch(buttonsGrid[i, j]))
                    {
                        buttonsToRemove.Add(buttonsGrid[i, j]);
                    }
                }
            }

            foreach (Button button in buttonsToRemove)
            {
                // Удаление кнопки из игрового поля
                button.Visible = false;
                score += 10; // Добавление очков
                scoreLabel.Text = $"{score}";
            }

            ShiftDownElements();
        }

        private List<int> GetSymbolOptions(Dictionary<int, string> symbols)
        {
            List<int> symbolOptions = new List<int>();
            foreach (var symbol in symbols)
            {
                symbolOptions.Add(symbol.Key);
            }
            return symbolOptions;
        }

        private void ShiftDownElements()
        {
            List<int> symbolOptions = GetSymbolOptions(symbols);
            Random random = new Random();

            for (int j = 0; j < gridSize; j++)
            {
                for (int i = gridSize - 1; i >= 0; i--)
                {
                    if (!buttonsGrid[i, j].Visible)
                    {
                        if (symbolOptions.Count == 0)
                        {

                            break;
                        }

                        int randomIndex = random.Next(0, symbolOptions.Count);
                        int newSymbolKey = symbolOptions[randomIndex];
                        string newSymbol = symbols[newSymbolKey];
                        symbolOptions.RemoveAt(randomIndex);
                        buttonsGrid[i, j].Tag = newSymbolKey;
                        buttonsGrid[i, j].Text = newSymbol;
                        buttonsGrid[i, j].Visible = true;
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            remainingTime--;
            timeLabel.Text = "Time : " + remainingTime + " s.";

            if (remainingTime == 0)
            {
                timer.Stop();
                MessageBox.Show("Game Over! Your score: " + score);
                OpenWindow();
            }

        }
        private void OpenWindow()
        {
            Form1 form1 = new Form1();
            form1.Show();

            this.Close();
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
