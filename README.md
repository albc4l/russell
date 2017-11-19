# Russell

This is the repository for a tool used to design a stocks portfolio based upon the ideas of Fred Piard, a quantitative investing expert.
Original article: https://seekingalpha.com/article/4124541-kiss-dividend-strategy-worth-15-percent-year-part-2=

The tool extracts the composition of the Russell 1000 ETF from iShares Website and then information for each ticker is fetched from IEX using their rest API. The strategy is based upon the dividend yield and the ROA of each stock. A rank is defined and the top entries are suggested as portfolio candidates. The tool generates an Excel file with two worksheets, one with the final portfolio selection and one with all discovered stocks for those interested into applying other filters/criterias. 

The tool also supports Swiss equities using SIX pages for latest metrics.

It is a console tool with user interaction.

## Main workflow for Russell 1000

1. Load data from iShares (the Russell 1000 current composition. See DownloadRussellComposition method)
2. Extract tickers
3. Load data from IEX for each ticker (metrics + company info)
4. Store these info into a "StockRepository" which is then saved on disk and used as a cache
nb. Steps (1-4) are done when "refreshing" the cache or if no cache is available because it's time-consuming
5. Read the cache, extract the stocks
6. Apply the strategy rules, get a list of stocks for the portfolio
7. Generate an Excel file with results

## Compiling

The solution can be opened and compiled with Visual Studio 2017.
