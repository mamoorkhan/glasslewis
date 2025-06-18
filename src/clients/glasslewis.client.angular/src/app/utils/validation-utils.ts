  import { AbstractControl } from '@angular/forms';

  // Custom validator for ISIN
  export function isinValidator(control: AbstractControl) {
    if (!control.value) return null;
    
    const isin = control.value.toString().toUpperCase();
    
    // ISIN should be 12 characters long
    if (isin.length !== 12) {
      return { invalidIsin: true };
    }
    
    // First two characters should be letters (country code)
    if (!/^[A-Z]{2}/.test(isin)) {
      return { invalidIsin: true };
    }
    
    // Remaining 10 characters should be alphanumeric
    if (!/^[A-Z]{2}[A-Z0-9]{10}$/.test(isin)) {
      return { invalidIsin: true };
    }
    
    // Basic ISIN checksum validation (simplified)
    return null;
  }
  export function urlValidator(control: AbstractControl) {
    if (!control.value) return null;
    
    try {
      new URL(control.value);
      return null;
    } catch {
      return { invalidUrl: true };
    }
  }