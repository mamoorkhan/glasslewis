export interface GetCompanyResponseModel {
  id: string; // Unique identifier for the company
  name: string; // Name of the company
  stockTicker: string; // Stock ticker symbol
  exchange: string; // Stock exchange name
  isin: string; // International Securities Identification Number
  website?: string; // Optional, company's website URL
  createdAt: Date; // Date and time when the company was created
  updatedAt: Date; // Date and time when the company was last updated
}