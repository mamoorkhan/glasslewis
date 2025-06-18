export interface UpdateCompanyRequestModel {
  name: string; // Required, max length: 200
  stockTicker: string; // Required, max length: 10
  exchange: string; // Required, max length: 100
  isin: string; // Required, must pass ISIN validation
  website?: string; // Optional, must be a valid URL, max length: 500
}