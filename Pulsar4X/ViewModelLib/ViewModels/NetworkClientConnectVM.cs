﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.SqlServer.Server;
using Pulsar4X.ECSLib;
using Pulsar4X.Networking;


namespace Pulsar4X.ViewModel
{
    public class NetworkClientConnectVM : IViewModel
    {
        private GameVM _gameVM;

        public string HostAddress { get; set; }
        public int HostPortNum { get; set; }

        private NetworkClient NetClient { get { return _gameVM.NetworkModule as NetworkClient; } }
        
        public string GameName { get; private set; }
        public ObservableCollection<string> ServerMessages { get; private set; }

        private string _factionName;
        public string FactionName {
            get { return _factionName; }
            set { _factionName = value; OnPropertyChanged(); }
        }

        private string _factionPassword;
        public string FactionPassword
        {
            get { return _factionPassword; } 
            set { _factionPassword = value;OnPropertyChanged(); }
        }

        //private bool _isConnectedToGame;
        //public bool IsConnectedToGame { get { return _isConnectedToGame; } set { IsConnectedToGame = value; OnPropertyChanged(); } }
        public bool IsConnectedToGame { get; set; }

        public NetworkClientConnectVM()
        {           
        }

        public NetworkClientConnectVM(GameVM gameVM)
        {
            _gameVM = gameVM; 
            NetworkClient netClient = new NetworkClient("localhost", 28888);            
            _gameVM.NetworkModule = netClient;
            IsConnectedToGame = netClient.HasFullDataset;
            PropertyChanged += new PropertyChangedEventHandler(OnConnectedToGameChanged);
            HostAddress = NetClient.HostAddress;
            HostPortNum = NetClient.PortNum;

            ServerMessages = NetClient.Messages;
            
            //GetFactions = GetFactions;
            
        }

        private void OnConnectedToGameChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "HasFullDataset" && NetClient.HasFullDataset)
            {
                _gameVM.Game = NetClient.Game;
                _gameVM.PlayerFaction = NetClient.CurrentFaction;
                _gameVM.Refresh();
                NetClient.NetTickEvent += _gameVM.OnClientTickEvent;
            }
        }

        public void OnConnect()
        {
            NetClient.ClientConnect();
           // GetFactions = GetFactions;
            Login = Login;
            NetClient.PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);
        }

        private void UpdateFactions()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("Factions"));
        }


        private ICommand _connectButton;
        public ICommand ConnectButton
        {
            get
            {
                return _connectButton ?? (_connectButton = new CommandHandler(OnConnect, true));
            }
        }

        //private ICommand _getFactions;
        //public ICommand GetFactions
        //{
        //    get
        //    {
        //        if (_gameVM != null)
        //            return _getFactions ?? (_getFactions = new CommandHandler(NetClient.SendFactionListRequest, true)); //NetClient.IsConnectedToServer) );
        //        return _getFactions ?? (_getFactions = new CommandHandler(null, false));
        //    }
        //    set { OnPropertyChanged();}
        //}


        private ICommand _login;
        public ICommand Login
        {
            get
            {
                if (_gameVM != null)
                    return _login ?? (_login = new CommandHandler(SendFactionReq, true));// NetClient.IsConnectedToServer));
                return _login ?? (_login = new CommandHandler(null, false));
            }
            set { OnPropertyChanged(); }
        }

        private void SendFactionReq()
        {
            NetClient.SendFactionDataRequest(FactionName, FactionPassword);
            FactionPassword = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Refresh(bool partialRefresh = false)
        {
            throw new NotImplementedException();
        }


    }
}