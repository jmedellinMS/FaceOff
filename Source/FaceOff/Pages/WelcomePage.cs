﻿using System;

using Xamarin;
using Xamarin.Forms;

using FaceOff.Shared;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace FaceOff
{
    public class WelcomePage : BaseContentPage<WelcomeViewModel>
    {
        #region Constant Fields
        readonly Entry _player1Entry, _player2Entry;
        readonly BounceButton _startGameButton;
        #endregion

        #region Constructors
        public WelcomePage()
        {
            var player1Label = new DarkBlueLabel { Text = "Player 1" };
            var player2Label = new DarkBlueLabel { Text = "Player 2" };

            _player1Entry = new Entry();
            _player1Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.Player1));

            _player2Entry = new Entry();
            _player2Entry.SetBinding(Entry.TextProperty, nameof(ViewModel.Player2));

            _startGameButton = new BounceButton
            {
                Margin = new Thickness(0, 20, 0, 0),
                Text = "Start"
            };

            PopulateAutomationIDs();
            PopulatePlaceholderText();
            ConfigureCustomReturnEffect();

            NavigationPage.SetBackButtonTitle(this, "");

            Title = "FaceOff";
            BackgroundColor = Color.FromHex("#91E2F4");

            Content = new StackLayout
            {
                Padding = 20,
                Children = {
                    player1Label,
                    _player1Entry,
                    player2Label,
                    _player2Entry,
                    _startGameButton
                }
            };
        }
        #endregion

        #region Methods
        protected override void SubscribeEventHandlers() => _startGameButton.Clicked += HandleStartGameButtonClicked;
        protected override void UnsubscribeEventHandlers() => _startGameButton.Clicked -= HandleStartGameButtonClicked;

        void ExecutePlayer1EntryReturnCommand(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(() => _player2Entry.Focus());

        void DisplayEmptyPlayerNameAlert(int playerNumber) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", $"Player {playerNumber} Name is Blank", "OK"));


        void HandleStartGameButtonClicked(object sender, EventArgs e)
        {
            var isPlayer1EntryTextEmpty = string.IsNullOrWhiteSpace(_player1Entry.Text);
            var isPlayer2EntryTextEmpty = string.IsNullOrWhiteSpace(_player2Entry.Text);

            if (isPlayer1EntryTextEmpty)
            {
                AnalyticsHelpers.Track(AnalyticsConstants.StartGameButtonTapped, AnalyticsConstants.StartGameButtonTappedStatus, AnalyticsConstants.Player1NameEmpty);
                DisplayEmptyPlayerNameAlert(1);
            }
            else if (isPlayer2EntryTextEmpty)
            {
                AnalyticsHelpers.Track(AnalyticsConstants.StartGameButtonTapped, AnalyticsConstants.StartGameButtonTappedStatus, AnalyticsConstants.Player2NameEmpty);
                DisplayEmptyPlayerNameAlert(2);
            }
            else
            {
                AnalyticsHelpers.Track(AnalyticsConstants.StartGameButtonTapped, AnalyticsConstants.StartGameButtonTappedStatus, AnalyticsConstants.GameStarted);
                Device.BeginInvokeOnMainThread(async () => await Navigation.PushAsync(new FaceOffPage()));
            }
        }

        void PopulateAutomationIDs()
        {
            _player1Entry.AutomationId = AutomationIdConstants.Player1Entry;
            _player2Entry.AutomationId = AutomationIdConstants.Player2Entry;
            _startGameButton.AutomationId = AutomationIdConstants.StartGameButton;
        }

        void PopulatePlaceholderText()
        {
            _player1Entry.Placeholder = PlaceholderConstants.WelcomePagePlaceholderText;
            _player2Entry.Placeholder = PlaceholderConstants.WelcomePagePlaceholderText;
        }

        void ConfigureCustomReturnEffect()
        {
            CustomReturnEffect.SetReturnType(_player1Entry, ReturnType.Next);
            CustomReturnEffect.SetReturnCommand(_player1Entry, new Command(() => _player2Entry.Focus()));

            CustomReturnEffect.SetReturnType(_player2Entry, ReturnType.Go);
            CustomReturnEffect.SetReturnCommand(_player2Entry, new Command(() => HandleStartGameButtonClicked(_startGameButton, EventArgs.Empty)));
        }
        #endregion
    }
}
