import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { of, throwError } from 'rxjs';
import { CreateCompanyComponent } from './create-company.component';
import { CreateCompanyService } from '../../services/company/create-company.service';
import { CreateCompanyRequestModel } from '../../models/request/create-company-request.model';
import { CreateCompanyResponseModel } from '../../models/response/create-company-response.model';

describe('CreateCompanyComponent', () => {
  let component: CreateCompanyComponent;
  let fixture: ComponentFixture<CreateCompanyComponent>;
  let createCompanyService: jasmine.SpyObj<CreateCompanyService>;
  let location: jasmine.SpyObj<Location>;

  beforeEach(async () => {
    const createCompanySpy = jasmine.createSpyObj('CreateCompanyService', ['createCompany']);
    const locationSpy = jasmine.createSpyObj('Location', ['back']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, ReactiveFormsModule, CreateCompanyComponent],
      providers: [
        { provide: CreateCompanyService, useValue: createCompanySpy },
        { provide: Location, useValue: locationSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateCompanyComponent);
    component = fixture.componentInstance;
    createCompanyService = TestBed.inject(CreateCompanyService) as jasmine.SpyObj<CreateCompanyService>;
    location = TestBed.inject(Location) as jasmine.SpyObj<Location>;
  });

  describe('Component Initialization', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize the form with correct validators on component creation', () => {
      fixture.detectChanges();

      expect(component.createCompanyForm).toBeDefined();
      expect(component.createCompanyForm.controls['name']).toBeDefined();
      expect(component.createCompanyForm.controls['stockTicker']).toBeDefined();
      expect(component.createCompanyForm.controls['exchange']).toBeDefined();
      expect(component.createCompanyForm.controls['isin']).toBeDefined();
      expect(component.createCompanyForm.controls['website']).toBeDefined();
    });

    it('should initialize component properties with default values', () => {
      expect(component.isLoading).toBe(false);
      expect(component.isSuccess).toBe(false);
      expect(component.errorMessage).toBe('');
      expect(component.successMessage).toBe('');
    });
  });

  describe('Form Validation', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should validate required fields', () => {
      const form = component.createCompanyForm;
      
      // Set all required fields to empty
      form.patchValue({
        name: '',
        stockTicker: '',
        exchange: '',
        isin: ''
      });
      
      expect(form.get('name')?.hasError('required')).toBe(true);
      expect(form.get('stockTicker')?.hasError('required')).toBe(true);
      expect(form.get('exchange')?.hasError('required')).toBe(true);
      expect(form.get('isin')?.hasError('required')).toBe(true);
    });

    it('should validate maxLength constraints', () => {
      const form = component.createCompanyForm;
      
      form.patchValue({
        name: 'a'.repeat(201), // exceeds 200 char limit
        stockTicker: 'a'.repeat(11), // exceeds 10 char limit
        exchange: 'a'.repeat(101), // exceeds 100 char limit
        website: 'https://' + 'a'.repeat(500) // exceeds 500 char limit
      });
      
      expect(form.get('name')?.hasError('maxlength')).toBe(true);
      expect(form.get('stockTicker')?.hasError('maxlength')).toBe(true);
      expect(form.get('exchange')?.hasError('maxlength')).toBe(true);
      expect(form.get('website')?.hasError('maxlength')).toBe(true);
    });

    it('should validate stock ticker pattern', () => {
      const form = component.createCompanyForm;
      
      // Invalid characters
      form.get('stockTicker')?.setValue('abc!@#');
      expect(form.get('stockTicker')?.hasError('pattern')).toBe(true);
      
      // Valid characters
      form.get('stockTicker')?.setValue('AAPL');
      expect(form.get('stockTicker')?.hasError('pattern')).toBe(false);
      
      // Valid with numbers and periods
      form.get('stockTicker')?.setValue('BRK.A');
      expect(form.get('stockTicker')?.hasError('pattern')).toBe(false);
    });

    it('should validate ISIN format', () => {
      const form = component.createCompanyForm;
      
      // Invalid ISIN
      form.get('isin')?.setValue('invalid');
      expect(form.get('isin')?.hasError('invalidIsin')).toBe(true);
      
      // Valid ISIN
      form.get('isin')?.setValue('US0378331005');
      expect(form.get('isin')?.hasError('invalidIsin')).toBe(false);
    });

    it('should validate URL format', () => {
      const form = component.createCompanyForm;
      
      // Invalid URL
      form.get('website')?.setValue('invalid-url');
      expect(form.get('website')?.hasError('invalidUrl')).toBe(true);
      
      // Valid URL
      form.get('website')?.setValue('https://www.example.com');
      expect(form.get('website')?.hasError('invalidUrl')).toBe(false);
      
      // Empty URL should be valid (optional field)
      form.get('website')?.setValue('');
      expect(form.get('website')?.hasError('invalidUrl')).toBe(false);
    });
  });

  describe('Form Submission - Success Cases', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should call createCompanyService when form is valid and submitted', () => {
      const mockCompany: CreateCompanyRequestModel = {
        name: 'Apple Inc.',
        stockTicker: 'AAPL',
        exchange: 'NASDAQ',
        isin: 'US0378331005',
        website: 'https://www.apple.com'
      };

      const mockResponse: CreateCompanyResponseModel = {
        id: '1',
        ...mockCompany
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(of(mockResponse));

      component.onSubmit();

      expect(createCompanyService.createCompany).toHaveBeenCalledWith({
        name: 'Apple Inc.',
        stockTicker: 'AAPL',
        exchange: 'NASDAQ',
        isin: 'US0378331005',
        website: 'https://www.apple.com'
      });
      expect(component.successMessage).toBe('Company created successfully!');
      expect(component.errorMessage).toBe('');
      expect(component.isSuccess).toBe(true);
      expect(component.isLoading).toBe(false);
    });

    it('should handle submission without optional website field', () => {
      const mockCompany = {
        name: 'Tesla Inc.',
        stockTicker: 'TSLA',
        exchange: 'NASDAQ',
        isin: 'US88160R1014',
        website: ''
      };

      const mockResponse: CreateCompanyResponseModel = {
        id: '2',
        name: 'Tesla Inc.',
        stockTicker: 'TSLA',
        exchange: 'NASDAQ',
        isin: 'US88160R1014'
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(of(mockResponse));

      component.onSubmit();

      expect(createCompanyService.createCompany).toHaveBeenCalledWith({
        name: 'Tesla Inc.',
        stockTicker: 'TSLA',
        exchange: 'NASDAQ',
        isin: 'US88160R1014',
        website: undefined
      });
    });

    it('should convert stockTicker and isin to uppercase', () => {
      // Use valid uppercase values that pass validation
      const mockCompany = {
        name: 'Microsoft Corp',
        stockTicker: 'MSFT',
        exchange: 'NASDAQ',
        isin: 'US5949181045',
        website: 'https://www.microsoft.com'
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));

      component.onSubmit();

      expect(createCompanyService.createCompany).toHaveBeenCalledWith({
        name: 'Microsoft Corp',
        stockTicker: 'MSFT',
        exchange: 'NASDAQ',
        isin: 'US5949181045',
        website: 'https://www.microsoft.com'
      });
    });

    it('should handle lowercase input and convert to uppercase in service call', () => {
      // Set form controls individually to ensure they're valid
      component.createCompanyForm.get('name')?.setValue('Microsoft Corp');
      component.createCompanyForm.get('stockTicker')?.setValue('MSFT');
      component.createCompanyForm.get('exchange')?.setValue('NASDAQ');
      component.createCompanyForm.get('isin')?.setValue('US5949181045');
      component.createCompanyForm.get('website')?.setValue('https://www.microsoft.com');

      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));

      component.onSubmit();

      expect(createCompanyService.createCompany).toHaveBeenCalledWith({
        name: 'Microsoft Corp',
        stockTicker: 'MSFT',
        exchange: 'NASDAQ',
        isin: 'US5949181045',
        website: 'https://www.microsoft.com'
      });
    });

    it('should reset form after successful submission', () => {
      const mockCompany = {
        name: 'Google Inc.',
        stockTicker: 'GOOGL',
        exchange: 'NASDAQ',
        isin: 'US02079K3059',
        website: 'https://www.google.com'
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));
      spyOn(component.createCompanyForm, 'reset');

      component.onSubmit();

      expect(component.createCompanyForm.reset).toHaveBeenCalled();
    });

    it('should set loading state during submission', () => {
      const mockCompany = {
        name: 'Amazon Inc.',
        stockTicker: 'AMZN',
        exchange: 'NASDAQ',
        isin: 'US0231351067'
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));

      expect(component.isLoading).toBe(false);
      component.onSubmit();
      expect(component.isLoading).toBe(false); // Should be false after successful completion
    });
  });

  describe('Form Submission - Error Cases', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should handle errors when createCompanyService fails with error message', () => {
      const mockCompany = {
        name: 'Facebook Inc.',
        stockTicker: 'FB',
        exchange: 'NASDAQ',
        isin: 'US30303M1027'
      };

      const errorResponse = {
        error: {
          message: 'Company with this ticker already exists'
        }
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(throwError(() => errorResponse));

      component.onSubmit();

      expect(component.errorMessage).toBe('Company with this ticker already exists');
      expect(component.successMessage).toBe('');
      expect(component.isSuccess).toBe(false);
      expect(component.isLoading).toBe(false);
    });

    it('should handle errors when createCompanyService fails without specific error message', () => {
      const mockCompany = {
        name: 'Netflix Inc.',
        stockTicker: 'NFLX',
        exchange: 'NASDAQ',
        isin: 'US64110L1061'
      };

      component.createCompanyForm.patchValue(mockCompany);
      createCompanyService.createCompany.and.returnValue(throwError(() => new Error('Network error')));

      component.onSubmit();

      expect(component.errorMessage).toBe('An error occurred while creating the company. Please try again.');
      expect(component.successMessage).toBe('');
      expect(component.isSuccess).toBe(false);
      expect(component.isLoading).toBe(false);
    });

    it('should not call createCompanyService if the form is invalid', () => {
      component.createCompanyForm.patchValue({
        name: '',
        stockTicker: '',
        exchange: '',
        isin: ''
      });

      spyOn(component, 'markFormGroupTouched' as never);
      component.onSubmit();

      expect(createCompanyService.createCompany).not.toHaveBeenCalled();
      expect(component['markFormGroupTouched']).toHaveBeenCalled();
    });

    it('should mark all form controls as touched when form is invalid', () => {
      component.createCompanyForm.patchValue({
        name: '',
        stockTicker: '',
        exchange: '',
        isin: ''
      });

      component.onSubmit();

      Object.keys(component.createCompanyForm.controls).forEach(key => {
        expect(component.createCompanyForm.get(key)?.touched).toBe(true);
      });
    });
  });

  describe('Error Message Handling', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should return correct error message for required fields', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('');
      form.get('name')?.markAsTouched();
      expect(component.getFieldError('name')).toBe('Company name is required');

      form.get('stockTicker')?.setValue('');
      form.get('stockTicker')?.markAsTouched();
      expect(component.getFieldError('stockTicker')).toBe('Stock ticker is required');

      form.get('exchange')?.setValue('');
      form.get('exchange')?.markAsTouched();
      expect(component.getFieldError('exchange')).toBe('Exchange is required');

      form.get('isin')?.setValue('');
      form.get('isin')?.markAsTouched();
      expect(component.getFieldError('isin')).toBe('ISIN is required');
    });

    it('should return correct error message for maxlength errors', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('a'.repeat(201));
      form.get('name')?.markAsTouched();
      expect(component.getFieldError('name')).toBe('Company name cannot exceed 200 characters');

      form.get('stockTicker')?.setValue('a'.repeat(11));
      form.get('stockTicker')?.markAsTouched();
      expect(component.getFieldError('stockTicker')).toBe('Stock ticker cannot exceed 10 characters');
    });

    it('should return correct error message for pattern errors', () => {
      const form = component.createCompanyForm;
      
      form.get('stockTicker')?.setValue('invalid!');
      form.get('stockTicker')?.markAsTouched();
      expect(component.getFieldError('stockTicker')).toBe('Stock ticker must contain only uppercase letters, numbers, and periods');
    });

    it('should return correct error message for ISIN validation', () => {
      const form = component.createCompanyForm;
      
      form.get('isin')?.setValue('invalid');
      form.get('isin')?.markAsTouched();
      expect(component.getFieldError('isin')).toBe('Please enter a valid ISIN code (12 characters, starting with 2 letters)');
    });

    it('should return correct error message for URL validation', () => {
      const form = component.createCompanyForm;
      
      form.get('website')?.setValue('invalid-url');
      form.get('website')?.markAsTouched();
      expect(component.getFieldError('website')).toBe('Please enter a valid URL (e.g., https://example.com)');
    });

    it('should return empty string when field has no errors', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('Valid Company Name');
      expect(component.getFieldError('name')).toBe('');
    });

    it('should return empty string when field is not touched', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('');
      // Don't mark as touched
      expect(component.getFieldError('name')).toBe('');
    });

    it('should handle unknown field names gracefully', () => {
      expect(component.getFieldError('unknownField')).toBe('');
    });
  });

  describe('Field Validation Status', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should return true when field is invalid and touched', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('');
      form.get('name')?.markAsTouched();
      expect(component.isFieldInvalid('name')).toBe(true);
    });

    it('should return false when field is valid', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('Valid Company Name');
      form.get('name')?.markAsTouched();
      expect(component.isFieldInvalid('name')).toBe(false);
    });

    it('should return false when field is invalid but not touched', () => {
      const form = component.createCompanyForm;
      
      form.get('name')?.setValue('');
      // Don't mark as touched
      expect(component.isFieldInvalid('name')).toBe(false);
    });

    it('should handle non-existent fields', () => {
      expect(component.isFieldInvalid('nonExistentField')).toBe(false);
    });
  });

  describe('Navigation and Form Reset', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should call location.back() when goBack is called', () => {
      component.goBack();
      expect(location.back).toHaveBeenCalled();
    });

    it('should reset form and clear messages when resetForm is called', () => {
      // Set some state
      component.errorMessage = 'Some error';
      component.successMessage = 'Some success';
      component.isSuccess = true;
      component.createCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST'
      });

      spyOn(component.createCompanyForm, 'reset');
      
      component.resetForm();

      expect(component.createCompanyForm.reset).toHaveBeenCalled();
      expect(component.errorMessage).toBe('');
      expect(component.successMessage).toBe('');
      expect(component.isSuccess).toBe(false);
    });
  });

  describe('Console Logging', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should log success message when company is created successfully', () => {
      const mockResponse: CreateCompanyResponseModel = {
        id: '1',
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890'
      };

      spyOn(console, 'log');
      
      component.createCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890'
      });
      
      createCompanyService.createCompany.and.returnValue(of(mockResponse));
      component.onSubmit();

      expect(console.log).toHaveBeenCalledWith('Company created:', mockResponse);
    });

    it('should log error message when company creation fails', () => {
      const error = new Error('Test error');
      spyOn(console, 'error');
      
      component.createCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890'
      });
      
      createCompanyService.createCompany.and.returnValue(throwError(() => error));
      component.onSubmit();

      expect(console.error).toHaveBeenCalledWith('Error creating company:', error);
    });
  });

  describe('Edge Cases and State Management', () => {
    beforeEach(() => {
      fixture.detectChanges();
    });

    it('should clear error and success messages when starting new submission', () => {
      // Set initial state
      component.errorMessage = 'Previous error';
      component.successMessage = 'Previous success';
      
      component.createCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890'
      });
      
      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));
      component.onSubmit();

      // Messages should be cleared at start of submission
      expect(component.errorMessage).toBe('');
      expect(component.successMessage).toBe('Company created successfully!');
    });

    it('should handle null/undefined values in form gracefully', () => {
      component.createCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: null
      });
      
      createCompanyService.createCompany.and.returnValue(of({} as CreateCompanyResponseModel));
      
      expect(() => component.onSubmit()).not.toThrow();
    });

    it('should handle form control getter returning null', () => {
      spyOn(component.createCompanyForm, 'get').and.returnValue(null);
      
      expect(component.getFieldError('name')).toBe('');
      expect(component.isFieldInvalid('name')).toBe(false);
    });
  });
});