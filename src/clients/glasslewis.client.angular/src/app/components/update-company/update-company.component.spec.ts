import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import { of, throwError, Subject } from 'rxjs';
import { UpdateCompanyComponent } from './update-company.component';
import { GetCompanyService } from '../../services/company/get-company.service';
import { UpdateCompanyService } from '../../services/company/update-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { UpdateCompanyResponseModel } from '../../models/response/update-company-response.model';

describe('UpdateCompanyComponent', () => {
  let component: UpdateCompanyComponent;
  let fixture: ComponentFixture<UpdateCompanyComponent>;
  let mockGetCompanyService: jasmine.SpyObj<GetCompanyService>;
  let mockUpdateCompanyService: jasmine.SpyObj<UpdateCompanyService>;
  let mockLocation: jasmine.SpyObj<Location>;
  let mockActivatedRoute: ActivatedRoute;
  let paramsSubject: Subject<Params>;

  const mockCompany: GetCompanyResponseModel = {
    id: '123',
    name: 'Test Company',
    stockTicker: 'TEST',
    exchange: 'NYSE',
    isin: 'US1234567890',
    website: 'https://test.com',
    createdAt: new Date('2023-01-01'),
    updatedAt: new Date('2023-12-01')
  };

  const mockUpdateResponse: UpdateCompanyResponseModel = {
    name: 'Updated Company',
    stockTicker: 'UPDT',
    exchange: 'NASDAQ',
    isin: 'US0987654321',
    website: 'https://updated.com',
  };

  beforeEach(async () => {
    paramsSubject = new Subject();
    
    mockGetCompanyService = jasmine.createSpyObj('GetCompanyService', ['getCompany']);
    mockUpdateCompanyService = jasmine.createSpyObj('UpdateCompanyService', ['editCompany']);
    mockLocation = jasmine.createSpyObj('Location', ['back']);
    
    mockActivatedRoute = {
      params: paramsSubject.asObservable()
    } as Partial<ActivatedRoute> as ActivatedRoute;

    await TestBed.configureTestingModule({
      imports: [UpdateCompanyComponent, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: GetCompanyService, useValue: mockGetCompanyService },
        { provide: UpdateCompanyService, useValue: mockUpdateCompanyService },
        { provide: Location, useValue: mockLocation },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateCompanyComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    paramsSubject.complete();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should create form and load company on init', () => {
      mockGetCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.ngOnInit();
      
      expect(component.updateCompanyForm).toBeDefined();
      expect(component.updateCompanyForm.get('name')).toBeTruthy();
      expect(component.updateCompanyForm.get('stockTicker')).toBeTruthy();
      expect(component.updateCompanyForm.get('exchange')).toBeTruthy();
      expect(component.updateCompanyForm.get('isin')).toBeTruthy();
      expect(component.updateCompanyForm.get('website')).toBeTruthy();
    });
  });

  describe('Form Creation and Validation', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should create form with proper validators', () => {
      const form = component.updateCompanyForm;
      
      // Test required fields
      expect(form.get('name')?.hasError('required')).toBeTrue();
      expect(form.get('stockTicker')?.hasError('required')).toBeTrue();
      expect(form.get('exchange')?.hasError('required')).toBeTrue();
      expect(form.get('isin')?.hasError('required')).toBeTrue();
      
      // Website is optional
      expect(form.get('website')?.hasError('required')).toBeFalse();
    });

    it('should validate max length', () => {
      const form = component.updateCompanyForm;
      
      // Test max length validation
      const longName = 'a'.repeat(201);
      form.get('name')?.setValue(longName);
      expect(form.get('name')?.errors?.['maxlength']).toBeTruthy();
      
      const validName = 'a'.repeat(200);
      form.get('name')?.setValue(validName);
      expect(form.get('name')?.errors?.['maxlength']).toBeFalsy();
    });

    it('should validate stock ticker max length', () => {
      const form = component.updateCompanyForm;
      
      form.get('stockTicker')?.setValue('VERYLONGTICKER');
      expect(form.get('stockTicker')?.errors?.['maxlength']).toBeTruthy();
      
      form.get('stockTicker')?.setValue('VALID');
      expect(form.get('stockTicker')?.errors?.['maxlength']).toBeFalsy();
    });

    it('should validate exchange max length', () => {
      const form = component.updateCompanyForm;
      
      const longExchange = 'a'.repeat(101);
      form.get('exchange')?.setValue(longExchange);
      expect(form.get('exchange')?.errors?.['maxlength']).toBeTruthy();
    });

    it('should validate ISIN format', () => {
      const form = component.updateCompanyForm;
      
      // Invalid ISIN
      form.get('isin')?.setValue('INVALID');
      expect(form.get('isin')?.errors).toBeTruthy();
      
      // Valid ISIN
      form.get('isin')?.setValue('US1234567890');
      expect(form.get('isin')?.hasError('pattern')).toBeFalsy();
    });

    it('should validate URL format', () => {
      const form = component.updateCompanyForm;
      
      // Invalid URL
      form.get('website')?.setValue('not-a-url');
      expect(form.get('website')?.errors).toBeTruthy();
      
      // Valid URL
      form.get('website')?.setValue('https://example.com');
      expect(form.get('website')?.errors).toBeNull();
      
      // Empty URL should be valid (optional field)
      form.get('website')?.setValue('');
      expect(form.get('website')?.errors).toBeNull();
    });

    it('should validate website max length', () => {
      const form = component.updateCompanyForm;
      
      const longUrl = 'https://' + 'a'.repeat(493) + '.com';
      form.get('website')?.setValue(longUrl);
      expect(form.get('website')?.errors?.['maxlength']).toBeTruthy();
    });
  });

  describe('loadCompany', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should load company successfully', fakeAsync(() => {
      mockGetCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      paramsSubject.next({ id: '123' });
      tick();

      expect(component.isLoading).toBeFalse();
      expect(component.company).toEqual(mockCompany);
      expect(component.errorMessage).toBe('');
      expect(component.updateCompanyForm.value).toEqual({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      });
    }));

    it('should handle missing company ID', () => {
      paramsSubject.next({});

      expect(component.errorMessage).toBe('Invalid company ID provided');
      expect(mockGetCompanyService.getCompany).not.toHaveBeenCalled();
      expect(component.isLoading).toBeFalse();
    });

    it('should handle null company ID', () => {
      paramsSubject.next({ id: null });

      expect(component.errorMessage).toBe('Invalid company ID provided');
      expect(mockGetCompanyService.getCompany).not.toHaveBeenCalled();
    });

    it('should handle 404 error', () => {
      const error = { status: 404 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      spyOn(console, 'error');
      
      paramsSubject.next({ id: '123' });

      expect(component.isLoading).toBeFalse();
      expect(component.errorMessage).toBe('Company not found');
      expect(console.error).toHaveBeenCalledWith('Error loading company:', error);
    });

    it('should handle connection error', () => {
      const error = { status: 0 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      spyOn(console, 'error');
      
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Unable to connect to the server. Please check your connection.');
    });

    it('should handle generic error with message', () => {
      const error = { status: 500, message: 'Server error' };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Error loading company details: Server error');
    });

    it('should handle generic error without message', () => {
      const error = { status: 500 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Error loading company details: Unknown error');
    });

    it('should set loading state while fetching', () => {
      const companySubject = new Subject<GetCompanyResponseModel>();
      mockGetCompanyService.getCompany.and.returnValue(companySubject.asObservable());
      
      paramsSubject.next({ id: '123' });
      
      expect(component.isLoading).toBeTrue();
      expect(component.company).toBeNull();
      
      // Complete the request
      companySubject.next(mockCompany);
      companySubject.complete();
      
      expect(component.isLoading).toBeFalse();
      expect(component.company).toEqual(mockCompany);
    });
  });

  describe('populateForm', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should populate form with company data', () => {
      component['populateForm'](mockCompany);

      expect(component.updateCompanyForm.value).toEqual({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      });
    });

    it('should handle null website', () => {
      const companyWithNullWebsite = { ...mockCompany, website: null };
      
      component['populateForm'](companyWithNullWebsite);

      expect(component.updateCompanyForm.get('website')?.value).toBe('');
    });

    it('should handle undefined website', () => {
      const companyWithUndefinedWebsite = { ...mockCompany, website: undefined };
      
      component['populateForm'](companyWithUndefinedWebsite);

      expect(component.updateCompanyForm.get('website')?.value).toBe('');
    });
  });

  describe('isFieldInvalid', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should return true for invalid dirty field', () => {
      const nameField = component.updateCompanyForm.get('name');
      nameField?.setValue('');
      nameField?.markAsDirty();
      
      expect(component.isFieldInvalid('name')).toBeTrue();
    });

    it('should return true for invalid touched field', () => {
      const nameField = component.updateCompanyForm.get('name');
      nameField?.setValue('');
      nameField?.markAsTouched();
      
      expect(component.isFieldInvalid('name')).toBeTrue();
    });

    it('should return false for valid field', () => {
      component.updateCompanyForm.get('name')?.setValue('Valid Name');
      
      expect(component.isFieldInvalid('name')).toBeFalse();
    });

    it('should return false for pristine untouched invalid field', () => {
      // Field is invalid (required) but not dirty or touched
      expect(component.isFieldInvalid('name')).toBeFalse();
    });

    it('should return false for non-existent field', () => {
      expect(component.isFieldInvalid('nonExistent')).toBeFalse();
    });
  });

  describe('onSubmit', () => {
    beforeEach(() => {
      component.ngOnInit();
      component.company = mockCompany;
      
      // Set valid form values
      component.updateCompanyForm.patchValue({
        name: 'Updated Company',
        stockTicker: 'UPDT',
        exchange: 'NASDAQ',
        isin: 'US0987654321',
        website: 'https://updated.com'
      });
    });

    it('should submit successfully', () => {
      mockUpdateCompanyService.editCompany.and.returnValue(of(mockUpdateResponse));
      spyOn(console, 'log');
      
      component.onSubmit();

      expect(mockUpdateCompanyService.editCompany).toHaveBeenCalledWith('123', {
        name: 'Updated Company',
        stockTicker: 'UPDT',
        exchange: 'NASDAQ',
        isin: 'US0987654321',
        website: 'https://updated.com'
      });
      expect(component.successMessage).toBe('Company updated successfully!');
      expect(component.isLoading).toBeFalse();
      expect(console.log).toHaveBeenCalledWith('Company updated:', mockUpdateResponse);
    });

    it('should handle empty website as undefined', () => {
      mockUpdateCompanyService.editCompany.and.returnValue(of(mockUpdateResponse));
      component.updateCompanyForm.get('website')?.setValue('');
      
      component.onSubmit();

      expect(mockUpdateCompanyService.editCompany).toHaveBeenCalledWith('123', jasmine.objectContaining({
        website: undefined
      }));
    });

    it('should handle submission error with error message', () => {
      mockUpdateCompanyService.editCompany.and.returnValue(
        throwError(() => ({ error: { message: 'Custom error message' } }))
      );
      spyOn(console, 'error');
      
      component.onSubmit();

      expect(component.errorMessage).toBe('Custom error message');
      expect(component.isLoading).toBeFalse();
      expect(console.error).toHaveBeenCalledWith('Error updating company:', jasmine.any(Object));
    });

    it('should handle submission error without error message', () => {
      mockUpdateCompanyService.editCompany.and.returnValue(
        throwError(() => ({ error: {} }))
      );
      
      component.onSubmit();

      expect(component.errorMessage).toBe('An error occurred while updating the company. Please try again.');
    });

    it('should handle submission error with no error object', () => {
      mockUpdateCompanyService.editCompany.and.returnValue(
        throwError(() => ({}))
      );
      
      component.onSubmit();

      expect(component.errorMessage).toBe('An error occurred while updating the company. Please try again.');
    });

    it('should mark all fields as touched when form is invalid', () => {
      // Make form invalid
      component.updateCompanyForm.reset();
      
      const fields = ['name', 'stockTicker', 'exchange', 'isin', 'website'];
      fields.forEach(field => {
        spyOn(component.updateCompanyForm.get(field)!, 'markAsTouched');
      });
      
      component.onSubmit();

      fields.forEach(field => {
        expect(component.updateCompanyForm.get(field)!.markAsTouched).toHaveBeenCalled();
      });
      expect(mockUpdateCompanyService.editCompany).not.toHaveBeenCalled();
    });

    it('should not submit if company id is missing', () => {
      component.company = null;
      
      component.onSubmit();

      expect(mockUpdateCompanyService.editCompany).not.toHaveBeenCalled();
    });

    it('should clear messages before submission', () => {
      component.successMessage = 'Previous success';
      component.errorMessage = 'Previous error';
      mockUpdateCompanyService.editCompany.and.returnValue(of(mockUpdateResponse));
      
      component.onSubmit();

      // Messages should be cleared before the request
      expect(component.successMessage).toBe('Company updated successfully!');
      expect(component.errorMessage).toBe('');
    });

    it('should set loading state during submission', () => {
      const updateSubject = new Subject<UpdateCompanyResponseModel>();
      mockUpdateCompanyService.editCompany.and.returnValue(updateSubject.asObservable());
      
      component.onSubmit();
      
      expect(component.isSubmitting).toBeTrue();
      
      // Complete the request
      updateSubject.next(mockUpdateResponse);
      updateSubject.complete();

      expect(component.isSubmitting).toBeFalse();
    });
  });

  describe('goBack', () => {
    it('should call location.back()', () => {
      component.goBack();
      expect(mockLocation.back).toHaveBeenCalled();
    });
  });

  describe('onReset', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should reset form and reload company', () => {
      mockGetCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      // Set form values
      component.updateCompanyForm.patchValue({
        name: 'Changed Name',
        stockTicker: 'CHG',
        exchange: 'CHANGED',
        isin: 'US1111111111',
        website: 'https://changed.com'
      });
      component.successMessage = 'Previous success';
      component.errorMessage = 'Previous error';
      
      component.onReset();
      
      // Form should be reset
      expect(component.updateCompanyForm.value).toEqual({
        name: null,
        stockTicker: null,
        exchange: null,
        isin: null,
        website: null
      });
      
      // Messages should be cleared
      expect(component.successMessage).toBe('');
      expect(component.errorMessage).toBe('');
      
      // Company should be reloaded when params emit
      paramsSubject.next({ id: '123' });
      expect(mockGetCompanyService.getCompany).toHaveBeenCalled();
    });
  });

  describe('Template Integration', () => {
    it('should display loading spinner when loading', () => {
      component.isLoading = true;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('.loading-container')).toBeTruthy();
      expect(compiled.querySelector('.spinner')).toBeTruthy();
      expect(compiled.textContent).toContain('Loading company data...');
    });

    it('should display form when not loading', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('form')).toBeTruthy();
      expect(compiled.querySelector('.loading-container')).toBeFalsy();
    });

    it('should show field as error when invalid', () => {
      component.ngOnInit();
      component.isLoading = false;
      fixture.detectChanges();
      
      const nameField = component.updateCompanyForm.get('name');
      nameField?.setValue('');
      nameField?.markAsTouched();
      fixture.detectChanges();
      
      const nameInput = fixture.nativeElement.querySelector('#name');
      expect(nameInput.classList.contains('error')).toBeTrue();
      
      const errorMessage = fixture.nativeElement.querySelector('.error-message');
      expect(errorMessage).toBeTruthy();
      expect(errorMessage.textContent).toContain('Company name is required');
    });

    it('should display all validation errors', () => {
      component.ngOnInit();
      component.isLoading = false;
      fixture.detectChanges();
      
      // Test max length error
      const nameField = component.updateCompanyForm.get('name');
      nameField?.setValue('a'.repeat(201));
      nameField?.markAsTouched();
      fixture.detectChanges();
      
      const errorMessage = fixture.nativeElement.querySelector('.form-group:nth-child(1) .error-message');
      expect(errorMessage.textContent).toContain('Company name cannot exceed 200 characters');
      
      // Test ISIN pattern error - Fixed approach
      const isinField = component.updateCompanyForm.get('isin');
      isinField?.setValue('INVALID');
      isinField?.markAsTouched();
      fixture.detectChanges();
      
      // Find the ISIN form group by looking for the input with id="isin"
      const isinInput = fixture.nativeElement.querySelector('#isin');
      const isinFormGroup = isinInput.closest('.form-group');
      const isinErrorMessage = isinFormGroup.querySelector('.error-message');
      
      expect(isinErrorMessage).toBeTruthy();
      expect(isinErrorMessage.textContent).toContain('Please enter a valid ISIN (12 characters, alphanumeric)');
    });

    it('should display success message', () => {
      component.successMessage = 'Update successful!';
      fixture.detectChanges();
      
      const successDiv = fixture.nativeElement.querySelector('.success-message');
      expect(successDiv).toBeTruthy();
      expect(successDiv.textContent).toContain('Update successful!');
    });

    it('should display error message', () => {
      component.errorMessage = 'Update failed!';
      fixture.detectChanges();
      
      const errorDiv = fixture.nativeElement.querySelector('.error-message-container');
      expect(errorDiv).toBeTruthy();
      expect(errorDiv.textContent).toContain('Update failed!');
    });

    it('should handle form submission', () => {
      component.ngOnInit();
      component.isLoading = false;
      component.company = mockCompany;
      
      // Fill form with valid data
      component.updateCompanyForm.patchValue({
        name: 'Updated Company',
        stockTicker: 'UPDT',
        exchange: 'NASDAQ',
        isin: 'US0987654321',
        website: 'https://updated.com'
      });
      
      fixture.detectChanges();
      
      spyOn(component, 'onSubmit');
      const form = fixture.nativeElement.querySelector('form');
      form.dispatchEvent(new Event('submit'));
      
      expect(component.onSubmit).toHaveBeenCalled();
    });

    it('should handle button clicks', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      // Test reset button
      spyOn(component, 'onReset');
      const resetButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[0];
      resetButton.click();
      expect(component.onReset).toHaveBeenCalled();
      
      // Test back button
      spyOn(component, 'goBack');
      const backButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[1];
      backButton.click();
      expect(component.goBack).toHaveBeenCalled();
    });

    it('should disable buttons when form is submitting', () => {
      component.ngOnInit();
      component.isLoading = false; // Form must be visible
      
      // Fill form with valid data
      component.updateCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      });
      
      // Set submitting state (this disables buttons while keeping form visible)
      component.isSubmitting = true;
      
      fixture.detectChanges();
      
      const submitButton = fixture.nativeElement.querySelector('.btn-primary');
      const resetButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[0];
      const backButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[1];
      
      // Now buttons exist and should be disabled
      expect(submitButton.disabled).toBeTrue();
      expect(resetButton.disabled).toBeTrue();
      expect(backButton.disabled).toBeTrue();
    });

    it('should disable submit button when form is invalid', () => {
      component.ngOnInit();
      component.isLoading = false;
      component.updateCompanyForm.reset(); // Make form invalid
      fixture.detectChanges();
      
      const submitButton = fixture.nativeElement.querySelector('.btn-primary');
      expect(submitButton.disabled).toBeTrue();
    });

    it('should show updating text on submit button during submission', () => {
      component.ngOnInit();
      component.isLoading = false;
      
      // Fill form with valid data
      component.updateCompanyForm.patchValue({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      });
      
      fixture.detectChanges();
      
      // Set submitting state directly
      component.isSubmitting = true;
      fixture.detectChanges();
      
      // Button should show "Updating..."
      const submitButton = fixture.nativeElement.querySelector('.btn-primary');
      expect(submitButton.textContent.trim()).toContain('Updating...');
    });


    it('should display required field indicators', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      const labels = fixture.nativeElement.querySelectorAll('label');
      expect(labels[0].textContent).toContain('Company Name *');
      expect(labels[1].textContent).toContain('Stock Ticker *');
      expect(labels[2].textContent).toContain('Exchange *');
      expect(labels[3].textContent).toContain('ISIN *');
      expect(labels[4].textContent).toContain('Website'); // No asterisk for optional field
    });
  });
});