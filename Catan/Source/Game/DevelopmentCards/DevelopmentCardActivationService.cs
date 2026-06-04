using System;
using System.Collections.Generic;
using Catan.Source.Game.Inventory;
using Catan.Source.Game.Resources;
using GameBank = Catan.Source.Game.Bank.Bank;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.DevelopmentCards
{
    public class DevelopmentCardActivationService
    {
        public DevelopmentCardActivationResult UseYearOfPlenty(
            GamePlayer player,
            GameBank bank,
            bool hasUsedDevelopmentCardThisTurn,
            ResourceId firstResource,
            ResourceId secondResource)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (bank == null)
            {
                throw new ArgumentNullException(nameof(bank));
            }

            DevelopmentCardActivationResult validation = CanUseYearOfPlenty(
                player,
                bank,
                hasUsedDevelopmentCardThisTurn,
                firstResource,
                secondResource);

            if (!validation.Success)
            {
                return validation;
            }

            player.Inventory.DevelopmentCards.Remove(new DevelopmentCard(DevelopmentCardType.YearOfPlenty));
            bank.Give(player.Inventory.Resources, firstResource, 1);
            bank.Give(player.Inventory.Resources, secondResource, 1);

            return DevelopmentCardActivationResult.Ok("Invenção executada.");
        }

        public DevelopmentCardActivationResult CanUseYearOfPlenty(
            GamePlayer player,
            GameBank bank,
            bool hasUsedDevelopmentCardThisTurn,
            ResourceId firstResource,
            ResourceId secondResource)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (bank == null)
            {
                throw new ArgumentNullException(nameof(bank));
            }

            if (hasUsedDevelopmentCardThisTurn)
            {
                return DevelopmentCardActivationResult.Fail("Já usou uma carta de desenvolvimento neste turno.");
            }

            if (player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.YearOfPlenty) <= 0)
            {
                return DevelopmentCardActivationResult.Fail("Jogador não possui Invenção jogável.");
            }

            if (firstResource == secondResource)
            {
                if (!bank.CanGive(firstResource, 2))
                {
                    return DevelopmentCardActivationResult.Fail("Banco não possui recursos suficientes para Invenção.");
                }

                return DevelopmentCardActivationResult.Ok("Invenção disponível.");
            }

            if (!bank.CanGive(firstResource, 1) || !bank.CanGive(secondResource, 1))
            {
                return DevelopmentCardActivationResult.Fail("Banco não possui recursos suficientes para Invenção.");
            }

            return DevelopmentCardActivationResult.Ok("Invenção disponível.");
        }

        public DevelopmentCardActivationResult UseMonopoly(
            GamePlayer activePlayer,
            IEnumerable<GamePlayer> players,
            bool hasUsedDevelopmentCardThisTurn,
            ResourceId resource)
        {
            if (activePlayer == null)
            {
                throw new ArgumentNullException(nameof(activePlayer));
            }

            if (players == null)
            {
                throw new ArgumentNullException(nameof(players));
            }

            DevelopmentCardActivationResult validation = CanUseMonopoly(
                activePlayer,
                players,
                hasUsedDevelopmentCardThisTurn,
                resource);

            if (!validation.Success)
            {
                return validation;
            }

            int total = 0;
            foreach (GamePlayer player in players)
            {
                if (player == null || ReferenceEquals(player, activePlayer))
                {
                    continue;
                }

                int amount = player.Inventory.Resources.GetAmount(resource);
                if (amount == 0)
                {
                    continue;
                }

                player.Inventory.Resources.Remove(resource, amount);
                total += amount;
            }

            activePlayer.Inventory.DevelopmentCards.Remove(new DevelopmentCard(DevelopmentCardType.Monopoly));
            activePlayer.Inventory.Resources.Add(resource, total);

            return DevelopmentCardActivationResult.Ok("Monopólio executado.");
        }

        public DevelopmentCardActivationResult CanUseMonopoly(
            GamePlayer activePlayer,
            IEnumerable<GamePlayer> players,
            bool hasUsedDevelopmentCardThisTurn,
            ResourceId resource)
        {
            if (activePlayer == null)
            {
                throw new ArgumentNullException(nameof(activePlayer));
            }

            if (players == null)
            {
                throw new ArgumentNullException(nameof(players));
            }
            if (hasUsedDevelopmentCardThisTurn)
            {
                return DevelopmentCardActivationResult.Fail("Já usou uma carta de desenvolvimento neste turno.");
            }

            if (activePlayer.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Monopoly) <= 0)
            {
                return DevelopmentCardActivationResult.Fail("Jogador não possui Monopólio jogável.");
            }

            foreach (GamePlayer player in players)
            {
                if (player == null)
                {
                    return DevelopmentCardActivationResult.Fail("Lista de jogadores inválida.");
                }

                player.Inventory.Resources.GetAmount(resource);
            }

            return DevelopmentCardActivationResult.Ok("Monopólio disponível.");
        }

        public DevelopmentCardActivationResult ConfirmKnightUse(
            GamePlayer player,
            bool hasUsedDevelopmentCardThisTurn)
        {
            DevelopmentCardActivationResult validation = CanUseKnight(player, hasUsedDevelopmentCardThisTurn);
            if (!validation.Success)
            {
                return validation;
            }

            player.Inventory.DevelopmentCards.Remove(new DevelopmentCard(DevelopmentCardType.Knight));
            player.Inventory.IncrementPlayedKnights();
            return DevelopmentCardActivationResult.Ok("Cavaleiro executado.");
        }

        public DevelopmentCardActivationResult CanUseKnight(
            GamePlayer player,
            bool hasUsedDevelopmentCardThisTurn)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }

            if (hasUsedDevelopmentCardThisTurn)
            {
                return DevelopmentCardActivationResult.Fail("Já usou uma carta de desenvolvimento neste turno.");
            }

            if (player.Inventory.DevelopmentCards.CountPlayableByType(DevelopmentCardType.Knight) <= 0)
            {
                return DevelopmentCardActivationResult.Fail("Jogador não possui Cavaleiro jogável.");
            }

            return DevelopmentCardActivationResult.Ok("Cavaleiro disponível.");
        }
    }
}
