# DeFi Portfolio & Yield Tracker

A robust backend service and web application for automatically tracking portfolio performance and yield across multiple DeFi protocols and blockchain networks. It provides consolidated, time-based financial reporting.

## 🚀 Features

- **Multi-Protocol Support**: Automatically fetches and aggregates data every 50 min from:
  - **Aave** (on Avalanche, Celo, and Sonic networks)
  - **Hyperliquid** (Perpetual DEX & L1)
  - **Uniswap** (DEX positions)

- **Multi-Wallet Management**: Track performance across multiple cryptocurrency wallet addresses simultaneously from a single dashboard.

- **Automated Data Synchronization**: The system polls supported platforms every 50 minutes to keep your portfolio data accurate and up-to-date without manual intervention.

- **Flexible Excel Reporting**: Generate and download detailed Excel (`.xlsx`) reports for any custom date range, enabling deep financial analysis and record-keeping.

## 🛠 Tech Stack

*   **Backend:** ASP.NET Core
*   **Data Aggregation:** Netherium + Uniswap api for Blockchain RPC calls
*   **Database:** PostgreSQL - for storing historical portfolio snapshots
*   **Task Scheduling:** TickerQ - for the 50-minute sync cycle
*   **Reporting:** Spreadcheetah for generating structured reports

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the issues page.

## 📄 License

This project is licensed under the [MIT License](LICENSE).
