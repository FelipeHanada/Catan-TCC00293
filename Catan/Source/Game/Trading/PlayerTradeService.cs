using System.Collections.Generic;
using Catan.Source.Game.Resources;
using GamePlayer = Catan.Source.Game.Player.Player;

namespace Catan.Source.Game.Trading
{
    public class PlayerTradeService
    {
        public PlayerTradeResult CanCreateOffer(
            GamePlayer offeringPlayer,
            IReadOnlyDictionary<ResourceId, int> offeredResources,
            IReadOnlyDictionary<ResourceId, int> requestedResources)
        {
            PlayerTradeResult validationResult = ValidateOfferShape(offeringPlayer, offeredResources, requestedResources);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            if (!offeringPlayer.Inventory.Resources.HasEnough(offeredResources))
            {
                return PlayerTradeResult.Fail($"Jogador {offeringPlayer.PlayerNumber} nao possui os recursos oferecidos.");
            }

            return PlayerTradeResult.Ok("Oferta valida.");
        }

        public PlayerTradeResult CreateOffer(
            GamePlayer offeringPlayer,
            IReadOnlyDictionary<ResourceId, int> offeredResources,
            IReadOnlyDictionary<ResourceId, int> requestedResources,
            out PlayerTradeOffer offer)
        {
            PlayerTradeResult result = CanCreateOffer(offeringPlayer, offeredResources, requestedResources);
            if (!result.Success)
            {
                offer = null;
                return result;
            }

            offer = new PlayerTradeOffer(offeringPlayer, offeredResources, requestedResources);
            return PlayerTradeResult.Ok($"Jogador {offeringPlayer.PlayerNumber} criou oferta de troca.");
        }

        public PlayerTradeResult CanAccept(PlayerTradeOffer offer, GamePlayer acceptingPlayer)
        {
            if (offer == null)
            {
                return PlayerTradeResult.Fail("Oferta nao pode ser nula.");
            }

            if (acceptingPlayer == null)
            {
                return PlayerTradeResult.Fail("Jogador aceitante nao pode ser nulo.");
            }

            PlayerTradeResult validationResult = ValidateOfferShape(
                offer.OfferingPlayer,
                offer.OfferedResources,
                offer.RequestedResources);
            if (!validationResult.Success)
            {
                return validationResult;
            }

            if (!offer.IsOpen)
            {
                return PlayerTradeResult.Fail("Oferta ja foi encerrada.");
            }

            if (acceptingPlayer == offer.OfferingPlayer)
            {
                return PlayerTradeResult.Fail("Jogador nao pode aceitar a propria oferta.");
            }

            if (!offer.OfferingPlayer.Inventory.Resources.HasEnough(offer.OfferedResources))
            {
                return PlayerTradeResult.Fail($"Jogador {offer.OfferingPlayer.PlayerNumber} nao possui mais os recursos oferecidos.");
            }

            if (!acceptingPlayer.Inventory.Resources.HasEnough(offer.RequestedResources))
            {
                return PlayerTradeResult.Fail($"Jogador {acceptingPlayer.PlayerNumber} nao possui os recursos pedidos.");
            }

            return PlayerTradeResult.Ok("Oferta pode ser aceita.");
        }

        public PlayerTradeResult Execute(PlayerTradeOffer offer, GamePlayer acceptingPlayer)
        {
            PlayerTradeResult result = CanAccept(offer, acceptingPlayer);
            if (!result.Success)
            {
                return result;
            }

            offer.OfferingPlayer.Inventory.Resources.Remove(offer.OfferedResources);
            acceptingPlayer.Inventory.Resources.Remove(offer.RequestedResources);
            offer.OfferingPlayer.Inventory.Resources.Add(offer.RequestedResources);
            acceptingPlayer.Inventory.Resources.Add(offer.OfferedResources);
            offer.Close();

            return PlayerTradeResult.Ok(
                $"Jogador {acceptingPlayer.PlayerNumber} aceitou a oferta do Jogador {offer.OfferingPlayer.PlayerNumber}.");
        }

        private PlayerTradeResult ValidateOfferShape( // validação da estrutura da oferta 
            GamePlayer offeringPlayer,
            IReadOnlyDictionary<ResourceId, int> offeredResources,
            IReadOnlyDictionary<ResourceId, int> requestedResources)
        {
            if (offeringPlayer == null)
            {
                return PlayerTradeResult.Fail("Jogador ofertante nao pode ser nulo.");
            }

            PlayerTradeResult offeredResult = ValidateResources(offeredResources, "oferecidos");
            if (!offeredResult.Success)
            {
                return offeredResult;
            }

            PlayerTradeResult requestedResult = ValidateResources(requestedResources, "pedidos");
            if (!requestedResult.Success)
            {
                return requestedResult;
            }

            foreach (ResourceId resource in offeredResources.Keys)
            {
                if (requestedResources.ContainsKey(resource))
                {
                    return PlayerTradeResult.Fail($"Recurso {resource} nao pode aparecer nos dois lados da troca.");
                }
            }

            return PlayerTradeResult.Ok("Oferta estruturalmente valida.");
        }

        private PlayerTradeResult ValidateResources(IReadOnlyDictionary<ResourceId, int> resources, string sideName)
        {
            if (resources == null)
            {
                return PlayerTradeResult.Fail($"Recursos {sideName} nao podem ser nulos.");
            }

            if (resources.Count == 0)
            {
                return PlayerTradeResult.Fail($"Recursos {sideName} nao podem ser vazios.");
            }

            foreach (var resource in resources)
            {
                if (!IsSupportedResource(resource.Key))
                {
                    return PlayerTradeResult.Fail($"Recurso {resource.Key} nao e suportado.");
                }

                if (resource.Value <= 0)
                {
                    return PlayerTradeResult.Fail($"Quantidade de {resource.Key} deve ser maior que zero.");
                }
            }

            return PlayerTradeResult.Ok($"Recursos {sideName} validos.");
        }

        private bool IsSupportedResource(ResourceId resource)
        {
            foreach (ResourceId supportedResource in System.Enum.GetValues<ResourceId>())
            {
                if (supportedResource == resource)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
