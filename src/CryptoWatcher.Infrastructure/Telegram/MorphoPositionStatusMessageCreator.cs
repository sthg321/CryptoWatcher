using System.Text;
using CryptoWatcher.Modules.Morpho.Application.Models;

namespace CryptoWatcher.Infrastructure.Telegram;

public class MorphoPositionStatusMessageCreator
{
    public static string CreateMessageFromModels(IEnumerable<MorphoPositionsStatus> positions)
    {
        var sb = new StringBuilder();

        foreach (var positionByWallet in positions.GroupBy(status => status.Wallet))
        {
            sb.AppendLine($"Кошелек: {positionByWallet.Key}");

            foreach (var positionByNetwork in positionByWallet.GroupBy(status => status.Network))
            {
                sb.AppendLine($"Сеть: {positionByNetwork.Key}");
                sb.AppendLine();

                foreach (var position in positionByNetwork)
                {
                    var hf = Math.Round(position.HealthFactor, 2);
                    var hfWarning = hf < 1.2 ? "⚠️" : hf < 1.5 ? "🟡" : "🟢";

                    sb.AppendLine($"{position.Collateral.Symbol}:");
                    sb.AppendLine(
                        $"Сумма: ${Math.Round(position.Collateral.AmountInUsd, 2)} ({Math.Round(position.Collateral.Amount, 2)})");
                    sb.AppendLine(
                        $"Цена: ${Math.Round(position.Collateral.PriceInUsd, 2)}. Ликвидация: ${Math.Round(position.CollateralLiquidationPrice, 2)}");
                    
                    sb.AppendLine($"{hfWarning} Health Factor: {hf}");
                    sb.AppendLine(new string('─', 30));
                }
            }
        }

        return sb.ToString();
    }
}