using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Uno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Deck _deck;
        private DiscardPile _discardPile;
        Player[] _players;
        private int _round = 1; //TODO See how to bind this to a label so it auto updates
        private int turn = 0;
        private Player _currentPlayer;

        public MainPage()
        {
            this.InitializeComponent();
            _deck = new Deck();
            _deck.Shuffle();
            _discardPile = new DiscardPile();
        }

        private void PlayGameButton_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            PlayerGrid.Visibility = Visibility.Visible;
            //Create players 
            //Start with 2 players and see if we have time for more
            //     (involves reversing player list or direction of play)
            var item = NumberOfPlayersComboBox.SelectedValue as ComboBoxItem;
            int numberOfPlayers = Convert.ToInt32(item.Content);
            _players = new Player[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                _players[i] = new Player($"Player {i + 1}");
            }

            //Deal cards
            _deck.Deal(_players);

            //Flip fist card to discard pile
            var firstCard = _deck.Draw();
            _discardPile.AddCard(firstCard);

            if (firstCard.GetType() == typeof(WildCard))
            {
                //TODO Player gets to pick any color and has to play a card on that color
                //or just let them play any card?
            }

            //Determine if first card flipped requires an action
            if (firstCard.GetType() == typeof(ActionCard))
            {
                //TODO Add logic if first card is action card
                //((ActionCard)firstCard).TakeAction(); //TODO rethink implementation
                if (((ActionCard)firstCard).Action == Action.DrawTwo)
                {
                    //Add two cards to players hand, MoveNext
                }
                else if (((ActionCard)firstCard).Action == Action.Skip)
                {
                    //MoveNext
                }
                else if (((ActionCard)firstCard).Action == Action.Reverse)
                {
                    //MoveNext
                }
                else if (((ActionCard)firstCard).Action == Action.DrawFour)
                {
                    //return card to deck, shuffle, flip top card
                }
            }

            //Update UI 

            _currentPlayer = _players[turn % _players.Length];

            RoundLabel.Text = "Round " + _round;
            DiscardPileLabel.Text = "Pile: " + _discardPile.LastCardPlayed().Name;
            PlayerNameLabel.Text = _currentPlayer.Name + " Hand";
            HandComboBox.ItemsSource = _currentPlayer.GetHand().Select(x => x.Name);
        }

        private async void PlayCardButton_Click(object sender, RoutedEventArgs e)
        {
            var cardName = HandComboBox.SelectedItem as string;
            if (cardName == null)
            {
                await new MessageDialog("Please select a card from the dropdown.").ShowAsync();
                return;
            }
            var hand = _currentPlayer.GetHand();
            var card = hand.First(x => x.Name == cardName);
            bool discardIsValid = false;
            if (await ValidPlay(card, _discardPile.LastCardPlayed()))
            {
                discardIsValid = hand.Remove(card);
            }
            else
            {
                return;
            }
            if (discardIsValid)
            {
                //TODO Check for and apply actions here???
                
                _discardPile.AddCard(card);
                var previousPlayer = _currentPlayer;
                turn++;
                _currentPlayer = _players[turn % _players.Length];
                
                var lastCardPlayed = _discardPile.LastCardPlayed();
                var type = lastCardPlayed.GetType();
                if (type.BaseType == typeof(ActionCard))
                {
                    if(type == typeof(ReverseCard) || type == typeof(SkipCard))
                    {
                        turn++;
                    }
                    else if (type == typeof(DrawTwoCard))
                    {
                        
                        _currentPlayer.TakeCard(_deck.Draw());
                        _currentPlayer.TakeCard(_deck.Draw());
                        turn++;
                    }
                    else if (type == typeof(WildDrawFourCard))
                    {
                        _currentPlayer.TakeCard(_deck.Draw());
                        _currentPlayer.TakeCard(_deck.Draw());
                        _currentPlayer.TakeCard(_deck.Draw());
                        _currentPlayer.TakeCard(_deck.Draw());
                    }
                    else
                    {
                        Console.WriteLine("Issue with action cards.");
                    }


                }

                DiscardPileLabel.Text = "Pile: " + lastCardPlayed.Name;
                PlayerNameLabel.Text = _currentPlayer.Name + " Hand";
                HandComboBox.ItemsSource = _currentPlayer.GetHand().Select(x => x.Name);
                if (previousPlayer.GetHand().Count() == 0)
                {
                    ScoreRound(previousPlayer, _players);
                    RoundLabel.Text = $"Round {++_round}";
                    //Reset everything

                    //new deck
                    _deck = new Deck();

                    //shuffle deck
                    _deck.Shuffle();

                    //deal
                    _deck.Deal(_players);
                    //TODO flip first card
                    throw new NotImplementedException("flip first card");
                }
                while (!CanPlay(lastCardPlayed, _currentPlayer.GetHand()))
                {
                    _currentPlayer.TakeCard(_deck.Draw());
                    HandComboBox.ItemsSource = _currentPlayer.GetHand().Select(x => x.Name);
                }
            }
            else
            {
                await new MessageDialog("Something went terribly wrong! Head for the hills!!!").ShowAsync();
            }

            //Note there should probably be a redunacy check to make sure the total is 108 cards

        }

        /// <summary>
        /// Check that the player has a playable card in their hand
        /// </summary>
        /// <param name="card"></param>
        /// <param name="getHand"></param>
        /// <returns>bool</returns>
        private static bool CanPlay(Card card, IList<Card> hand)
        {
            var canPlay = false;

            var cardType = card.GetType();
            var cardBaseType = cardType.BaseType;

            //if card is color card look for those
            if (cardType != typeof(WildCard) && cardBaseType != typeof(WildCard))
            {
                if (cardType == typeof(NumberCard))
                {
                    foreach (Card cardInHand in hand)
                    {
                        var cardInHandType = cardInHand.GetType();
                        var cardInHandBaseType = cardInHandType.BaseType;

                        if (cardInHandBaseType == typeof(WildCard) || cardInHandType == typeof(WildCard))
                        {
                            canPlay = true;
                            break;
                        }
                        else if (cardInHandType == typeof(NumberCard))
                        {
                            if (((ColorCard)cardInHand).Color == ((ColorCard)card).Color)
                            {
                                canPlay = true;
                                break;
                            }
                            else if (((NumberCard)cardInHand).Number == ((NumberCard)card).Number)
                            {
                                canPlay = true;
                                break;
                            }
                        }
                        else if (cardInHandBaseType == typeof(ActionCard))
                        {
                            if (((ColorCard)cardInHand).Color == ((ColorCard)card).Color)
                            {
                                canPlay = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (Card cardInHand in hand)
                    {
                        var cardInHandType = cardInHand.GetType();
                        var cardInHandBaseType = cardInHandType.BaseType;

                        if (cardInHandBaseType == typeof(WildCard) || cardInHandType == typeof(WildCard))
                        {
                            canPlay = true;
                            break;
                        }
                        else if (cardInHandBaseType == typeof(ActionCard))
                        {
                            if (((ColorCard)cardInHand).Color == ((ColorCard)card).Color)
                            {
                                canPlay = true;
                                break;
                            }
                            else if (((ActionCard)cardInHand).Action == ((ActionCard)card).Action)
                            {
                                canPlay = true;
                                break;
                            }
                        }
                        else if (cardInHandType == typeof(NumberCard))
                        {
                            if (((ColorCard)cardInHand).Color == ((ColorCard)card).Color)
                            {
                                canPlay = true;
                                break;
                            }
                        }
                    }
                }
            }
            else if (cardType == typeof(WildCard) || cardBaseType == typeof(WildCard))
            {
                canPlay = true;
            }

            return canPlay;
        }

        /// <summary>
        /// Check that the card being played is valid
        /// </summary>
        /// <param name="card"></param>
        /// <param name="lastCardPlayed"></param>
        /// <returns>bool</returns>
        private static async Task<bool> ValidPlay(Card card, Card lastCardPlayed)
        {
            var cardType = card.GetType();
            var lastCardPlayedType = lastCardPlayed.GetType();
            Debug.WriteLine($"\nValidPlay()");
            Debug.WriteLine($"Card Type {cardType}");
            Debug.WriteLine($"Card Base Type {cardType.BaseType}");
            Debug.WriteLine($"LastCardPlayed Type {lastCardPlayedType}");
            Debug.WriteLine($"LastCardPlayed Base Type {lastCardPlayedType.BaseType}");
            if (cardType == typeof(WildCard)
                || cardType.BaseType == typeof(WildCard)
                || lastCardPlayedType == typeof(WildCard)
                || lastCardPlayedType.BaseType == typeof(WildCard))
            {
                //TODO Check if they have any other valid cards if it is a draw four
                //Stretch Goal: Introduce concept of challenge
                return true;
            }
            else if (cardType == typeof(NumberCard))
            {
                if (lastCardPlayedType == typeof(NumberCard))
                {
                    if (((ColorCard)lastCardPlayed).Color == ((ColorCard)card).Color)
                    {
                        return true;
                    }
                    else if (((NumberCard)lastCardPlayed).Number == ((NumberCard)card).Number)
                    {
                        return true;
                    }
                }
                else if (lastCardPlayedType.BaseType == typeof(ActionCard))
                {
                    if (((ColorCard)lastCardPlayed).Color == ((ColorCard)card).Color)
                    {
                        return true;
                    }
                }
            }
            else if (cardType.BaseType == typeof(ActionCard))
            {

                if (lastCardPlayedType.BaseType == typeof(ActionCard))
                {
                    if (((ColorCard)lastCardPlayed).Color == ((ColorCard)card).Color)
                    {
                        return true;
                    }
                    else if (((ActionCard)lastCardPlayed).Action == ((ActionCard)card).Action)
                    {
                        return true;
                    }
                }
                else if (lastCardPlayedType == typeof(NumberCard))
                {
                    if (((ColorCard)lastCardPlayed).Color == ((ColorCard)card).Color)
                    {
                        return true;
                    }
                }
            }
            await new MessageDialog("Invalid card. Please try again.").ShowAsync();
            return false;
        }

        /// <summary>
        /// Sets the score for the round after a player goes out.
        /// </summary>
        /// <param name="winner">Player to recieve points.</param>
        /// <param name="players">Players that contribute to score. Winner does not contribute, 
        /// since their hand is empty</param>
        private void ScoreRound(Player winner, ICollection<Player> players)
        {
            var points = 0;
            foreach (var player in players)
            {
                points += player.GetHand().Sum(x => x.Value);
            }
            winner.Score = points;
            _round++;
        }
    }
}
