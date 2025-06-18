export interface PatchCompanyRequestModel {
  name?: string; // Optional, max length: 200
  stockTicker?: string; // Optional, max length: 10
  exchange?: string; // Optional, max length: 100
  isin?: string; // Optional, must pass ISIN validation
  website?: string; // Optional, must be a valid URL, max length: 500
}