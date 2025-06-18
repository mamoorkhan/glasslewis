import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError, Subject } from 'rxjs';
import { ChangeDetectorRef } from '@angular/core';
import { PatchCompanyComponent } from './patch-company.component';
import { GetCompanyService } from '../../services/company/get-company.service';
import { PatchCompanyService } from '../../services/company/patch-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { PatchCompanyResponseModel } from '../../models/response/patch-company-response.model';
import { Params } from '@angular/router';

describe('PatchCompanyComponent', () => {
  let component: PatchCompanyComponent;
  let fixture: ComponentFixture<PatchCompanyComponent>;
  let mockGetCompanyService: jasmine.SpyObj<GetCompanyService>;
  let mockPatchCompanyService: jasmine.SpyObj<PatchCompanyService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: Partial<ActivatedRoute>;
  let mockChangeDetectorRef: jasmine.SpyObj<ChangeDetectorRef>;
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

  const mockPatchResponse: PatchCompanyResponseModel = {
    name: 'Updated Company',
    stockTicker: 'UPDT',
    exchange: 'NASDAQ',
    isin: 'US0987654321',
    website: 'https://updated.com',
  };

  beforeEach(async () => {
    paramsSubject = new Subject();
    
    mockGetCompanyService = jasmine.createSpyObj('GetCompanyService', ['getCompany']);
    mockPatchCompanyService = jasmine.createSpyObj('PatchCompanyService', ['patchCompany']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockChangeDetectorRef = jasmine.createSpyObj('ChangeDetectorRef', ['detectChanges']);
    
    mockActivatedRoute = {
      params: paramsSubject.asObservable()
    };

    await TestBed.configureTestingModule({
      imports: [PatchCompanyComponent, ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: GetCompanyService, useValue: mockGetCompanyService },
        { provide: PatchCompanyService, useValue: mockPatchCompanyService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PatchCompanyComponent);
    component = fixture.componentInstance;
    
    // Override the injected ChangeDetectorRef with our mock
    (component as PatchCompanyComponent)['cdr'] = mockChangeDetectorRef;
  });

  afterEach(() => {
    paramsSubject.complete();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load company on init', () => {
      mockGetCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(mockGetCompanyService.getCompany).toHaveBeenCalledWith('123');
      expect(component.company).toEqual(mockCompany);
      expect(component.isLoading).toBeFalse();
    });
  });

  describe('ngOnDestroy', () => {
    it('should complete destroy subject', () => {
      const destroySubject = component['destroy$'];
      spyOn(destroySubject, 'next');
      spyOn(destroySubject, 'complete');
      
      component.ngOnDestroy();

      expect(destroySubject.next).toHaveBeenCalled();
      expect(destroySubject.complete).toHaveBeenCalled();
    });
  });

  describe('Form Creation and Validation', () => {
    it('should create form with proper validators', () => {
      const form = component.patchCompanyForm;
      
      expect(form.get('name')).toBeTruthy();
      expect(form.get('stockTicker')).toBeTruthy();
      expect(form.get('exchange')).toBeTruthy();
      expect(form.get('isin')).toBeTruthy();
      expect(form.get('website')).toBeTruthy();
    });

    it('should validate required fields with whitespace', () => {
      const form = component.patchCompanyForm;
      
      // Test whitespace validation
      form.get('name')?.setValue('   ');
      expect(form.get('name')?.errors?.['requiredNoWhitespace']).toBeTruthy();
      
      form.get('name')?.setValue('Valid Name');
      expect(form.get('name')?.errors).toBeNull();
    });

    it('should validate max length', () => {
      const form = component.patchCompanyForm;
      
      // Test max length validation
      const longName = 'a'.repeat(201);
      form.get('name')?.setValue(longName);
      expect(form.get('name')?.errors?.['maxlength']).toBeTruthy();
      
      const validName = 'a'.repeat(200);
      form.get('name')?.setValue(validName);
      expect(form.get('name')?.errors).toBeNull();
    });

    it('should validate ISIN format', () => {
      const form = component.patchCompanyForm;
      
      // Invalid ISIN
      form.get('isin')?.setValue('INVALID');
      expect(form.get('isin')?.errors).toBeTruthy();
      
      // Valid ISIN
      form.get('isin')?.setValue('US1234567890');
      expect(form.get('isin')?.errors).toBeNull();
    });

    it('should validate URL format', () => {
      const form = component.patchCompanyForm;
      
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
  });

  describe('loadCompany', () => {
    it('should load company successfully', () => {
      mockGetCompanyService.getCompany.and.returnValue(of(mockCompany));
      spyOn(console, 'log');
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(component.isLoading).toBeFalse();
      expect(component.company).toEqual(mockCompany);
      expect(component.errorMessage).toBe('');
      expect(console.log).toHaveBeenCalledWith('Company data received:', mockCompany);
    });

    it('should handle missing company ID', () => {
      component.ngOnInit();
      paramsSubject.next({});

      expect(component.errorMessage).toBe('Invalid company ID provided');
      expect(mockGetCompanyService.getCompany).not.toHaveBeenCalled();
    });

    it('should handle 404 error', () => {
      const error = { status: 404 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      spyOn(console, 'error');
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(component.isLoading).toBeFalse();
      expect(component.errorMessage).toBe('Company not found');
      expect(console.error).toHaveBeenCalledWith('Error loading company:', error);
    });

    it('should handle connection error', () => {
      const error = { status: 0 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Unable to connect to the server. Please check your connection.');
    });

    it('should handle generic error with message', () => {
      const error = { status: 500, message: 'Server error' };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Error loading company details: Server error');
    });

    it('should handle generic error without message', () => {
      const error = { status: 500 };
      mockGetCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(component.errorMessage).toBe('Error loading company details: Unknown error');
    });
  });

  describe('populateForm', () => {
    it('should populate form with company data', () => {
      spyOn(console, 'log');
      
      component['populateForm'](mockCompany);

      expect(component.patchCompanyForm.value).toEqual({
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      });
      expect(component.patchCompanyForm.pristine).toBeTrue();
      expect(mockChangeDetectorRef.detectChanges).toHaveBeenCalled();
    });

    it('should handle null values in company data', () => {
      const companyWithNulls = {
        ...mockCompany,
        website: null
      };
      
      component['populateForm'](companyWithNulls);

      expect(component.patchCompanyForm.get('website')?.value).toBe('');
    });
  });

  describe('getChangedFields', () => {
    beforeEach(() => {
      component['originalFormData'] = {
        name: 'Original Name',
        stockTicker: 'ORIG',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://original.com'
      };
      component.patchCompanyForm.patchValue(component['originalFormData']);
    });

    it('should detect changed fields', () => {
      component.patchCompanyForm.get('name')?.setValue('New Name');
      component.patchCompanyForm.get('stockTicker')?.setValue('NEW');
      
      const changes = component['getChangedFields']();
      
      expect(changes).toEqual({
        name: 'New Name',
        stockTicker: 'NEW'
      });
    });

    it('should handle cleared fields', () => {
      component.patchCompanyForm.get('website')?.setValue('');
      
      const changes = component['getChangedFields']();
      
      expect(changes).toEqual({
        website: null
      });
    });

    it('should ignore unchanged fields', () => {
      const changes = component['getChangedFields']();
      expect(changes).toEqual({});
    });

    it('should trim whitespace when comparing', () => {
      component.patchCompanyForm.get('name')?.setValue('  Original Name  ');
      
      const changes = component['getChangedFields']();
      expect(changes).toEqual({});
    });
  });

  describe('hasChanges', () => {
    beforeEach(() => {
      component['originalFormData'] = {
        name: 'Original Name',
        stockTicker: 'ORIG'
      };
      component.patchCompanyForm.patchValue(component['originalFormData']);
    });

    it('should return true when fields have changed', () => {
      component.patchCompanyForm.get('name')?.setValue('New Name');
      expect(component.hasChanges()).toBeTrue();
    });

    it('should return false when no fields have changed', () => {
      expect(component.hasChanges()).toBeFalse();
    });
  });

  describe('isFieldInvalid', () => {
    it('should return true for invalid dirty field', () => {
      const nameField = component.patchCompanyForm.get('name');
      nameField?.setValue('');
      nameField?.markAsDirty();
      
      expect(component.isFieldInvalid('name')).toBeTrue();
    });

    it('should return true for invalid touched field', () => {
      const nameField = component.patchCompanyForm.get('name');
      nameField?.setValue('');
      nameField?.markAsTouched();
      
      expect(component.isFieldInvalid('name')).toBeTrue();
    });

    it('should return false for valid field', () => {
      component.patchCompanyForm.get('name')?.setValue('Valid Name');
      
      expect(component.isFieldInvalid('name')).toBeFalse();
    });

    it('should return false for non-existent field', () => {
      expect(component.isFieldInvalid('nonExistent')).toBeFalse();
    });
  });

  describe('onSubmit', () => {
    beforeEach(() => {
      component.company = mockCompany;
      component['originalFormData'] = {
        name: 'Original Name',
        stockTicker: 'ORIG',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://original.com'
      };
      component.patchCompanyForm.patchValue(component['originalFormData']);
    });

    it('should submit successfully with changes', () => {
      mockPatchCompanyService.patchCompany.and.returnValue(of(mockPatchResponse));
      spyOn(console, 'log');
      
      component.patchCompanyForm.get('name')?.setValue('New Name');
      component.onSubmit();

      expect(mockPatchCompanyService.patchCompany).toHaveBeenCalledWith('123', { name: 'New Name' });
      expect(component.successMessage).toBe('Company updated successfully!');
      expect(component.isSubmitting).toBeFalse();
      expect(component.patchCompanyForm.pristine).toBeTrue();
    });

    it('should show error when no changes detected', () => {
      component.onSubmit();

      expect(component.errorMessage).toBe('No changes detected. Please modify at least one field.');
      expect(mockPatchCompanyService.patchCompany).not.toHaveBeenCalled();
    });

    it('should handle submission error with error message', () => {
      mockPatchCompanyService.patchCompany.and.returnValue(
        throwError(() => ({ error: { message: 'Custom error message' } }))
      );
      spyOn(console, 'error');
      
      component.patchCompanyForm.get('name')?.setValue('New Name');
      component.onSubmit();

      expect(component.errorMessage).toBe('Custom error message');
      expect(component.isSubmitting).toBeFalse();
    });

    it('should handle submission error without error message', () => {
      mockPatchCompanyService.patchCompany.and.returnValue(
        throwError(() => ({ error: {} }))
      );
      
      component.patchCompanyForm.get('name')?.setValue('New Name');
      component.onSubmit();

      expect(component.errorMessage).toBe('An error occurred while updating the company. Please try again.');
    });

    it('should mark all fields as touched when form is invalid', () => {
      component.patchCompanyForm.get('name')?.setValue('   '); // Invalid whitespace
      component.patchCompanyForm.get('name')?.markAsDirty(); // Make it count as a change
      
      const fields = ['name', 'stockTicker', 'exchange', 'isin', 'website'];
      fields.forEach(field => {
        spyOn(component.patchCompanyForm.get(field)!, 'markAsTouched');
      });
      
      component.onSubmit();

      fields.forEach(field => {
        expect(component.patchCompanyForm.get(field)!.markAsTouched).toHaveBeenCalled();
      });
    });

    it('should not submit if company id is missing', () => {
      component.company = null;
      component.patchCompanyForm.get('name')?.setValue('New Name');
      
      component.onSubmit();

      expect(mockPatchCompanyService.patchCompany).not.toHaveBeenCalled();
    });
  });

  describe('onCancel', () => {
    it('should navigate to companies list', () => {
      component.onCancel();
      expect(mockRouter.navigate).toHaveBeenCalledWith(['/companies']);
    });
  });

  describe('onReset', () => {
    it('should reset form to original values', () => {
      component['originalFormData'] = {
        name: 'Original Name',
        stockTicker: 'ORIG',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://original.com'
      };
      
      component.patchCompanyForm.patchValue({
        name: 'Changed Name',
        stockTicker: 'CHG',
        exchange: 'NASDAQ',
        isin: 'US0987654321',
        website: 'https://changed.com'
      });
      
      component.successMessage = 'Previous success';
      component.errorMessage = 'Previous error';
      
      component.onReset();

      expect(component.patchCompanyForm.value).toEqual(component['originalFormData']);
      expect(component.patchCompanyForm.pristine).toBeTrue();
      expect(component.successMessage).toBe('');
      expect(component.errorMessage).toBe('');
    });
  });

  describe('clearField', () => {
    it('should clear field value and mark as dirty', () => {
      component.patchCompanyForm.get('name')?.setValue('Test Value');
      
      component.clearField('name');

      expect(component.patchCompanyForm.get('name')?.value).toBe('');
      expect(component.patchCompanyForm.get('name')?.dirty).toBeTrue();
    });
  });

  describe('restoreField', () => {
    it('should restore original field value', () => {
      component['originalFormData'] = { name: 'Original Name' };
      component.patchCompanyForm.get('name')?.setValue('Changed Name');
      component.patchCompanyForm.get('name')?.markAsDirty();
      
      component.restoreField('name');

      expect(component.patchCompanyForm.get('name')?.value).toBe('Original Name');
      expect(component.patchCompanyForm.get('name')?.pristine).toBeTrue();
    });

    it('should handle missing original value', () => {
      component['originalFormData'] = {};
      component.patchCompanyForm.get('name')?.setValue('Some Value');
      
      component.restoreField('name');

      expect(component.patchCompanyForm.get('name')?.value).toBe('');
    });
  });

  describe('Template Integration', () => {
    it('should display loading spinner when loading', () => {
      component.isLoading = true;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('.loading-container')).toBeTruthy();
      expect(compiled.querySelector('.spinner')).toBeTruthy();
    });

    it('should display form when not loading', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('form')).toBeTruthy();
      expect(compiled.querySelector('.info-banner')).toBeTruthy();
    });

    it('should show field as modified when dirty', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      component.patchCompanyForm.get('name')?.setValue('New Value');
      component.patchCompanyForm.get('name')?.markAsDirty();
      fixture.detectChanges();
      
      const nameInput = fixture.nativeElement.querySelector('#name');
      expect(nameInput.classList.contains('modified')).toBeTrue();
    });

    it('should show field as error when invalid', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      component.patchCompanyForm.get('name')?.setValue('');
      component.patchCompanyForm.get('name')?.markAsTouched();
      fixture.detectChanges();
      
      const nameInput = fixture.nativeElement.querySelector('#name');
      expect(nameInput.classList.contains('error')).toBeTrue();
    });

    it('should display changes summary when there are changes', () => {
      component.isLoading = false;
      component['originalFormData'] = { name: 'Original' };
      component.patchCompanyForm.patchValue({ name: 'Original' });
      fixture.detectChanges();
      
      // No changes initially
      expect(fixture.nativeElement.querySelector('.changes-summary')).toBeFalsy();
      
      // Make a change
      component.patchCompanyForm.get('name')?.setValue('Changed');
      component.patchCompanyForm.get('name')?.markAsDirty();
      fixture.detectChanges();
      
      const changesSummary = fixture.nativeElement.querySelector('.changes-summary');
      expect(changesSummary).toBeTruthy();
      expect(changesSummary.textContent).toContain('Company Name: Changed');
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

    it('should handle button clicks', () => {
      component.isLoading = false;
      component.company = mockCompany;
      
      // Set up form with valid data and changes
      component['originalFormData'] = {
        name: 'Original Name',
        stockTicker: 'ORIG',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://original.com'
      };
      
      // Populate form with all required fields
      component.patchCompanyForm.patchValue({
        name: 'Changed Name',
        stockTicker: 'CHG',
        exchange: 'NASDAQ',
        isin: 'US0987654321',
        website: 'https://changed.com'
      });
      
      // Mark name as dirty to indicate changes
      component.patchCompanyForm.get('name')?.markAsDirty();
      
      fixture.detectChanges();
      
      // Test submit button
      spyOn(component, 'onSubmit');
      const form = fixture.nativeElement.querySelector('form');
      form.dispatchEvent(new Event('submit'));
      expect(component.onSubmit).toHaveBeenCalled();
      
      // Test reset button
      spyOn(component, 'onReset');
      const resetButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[0];
      resetButton.click();
      expect(component.onReset).toHaveBeenCalled();
      
      // Test cancel button
      spyOn(component, 'onCancel');
      const cancelButton = fixture.nativeElement.querySelectorAll('.btn-secondary')[1];
      cancelButton.click();
      expect(component.onCancel).toHaveBeenCalled();
    });

    it('should handle field action buttons', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      // Test clear button
      spyOn(component, 'clearField');
      const clearButton = fixture.nativeElement.querySelector('.clear-btn');
      clearButton.click();
      expect(component.clearField).toHaveBeenCalledWith('name');
      
      // Test restore button
      spyOn(component, 'restoreField');
      const restoreButton = fixture.nativeElement.querySelector('.restore-btn');
      restoreButton.click();
      expect(component.restoreField).toHaveBeenCalledWith('name');
    });

    it('should disable submit button appropriately', () => {
      component.isLoading = false;
      component.company = mockCompany;
      
      // Set up form with original data (no changes)
      component['originalFormData'] = {
        name: 'Test Company',
        stockTicker: 'TEST',
        exchange: 'NYSE',
        isin: 'US1234567890',
        website: 'https://test.com'
      };
      
      // Populate form with same values as original (no changes)
      component.patchCompanyForm.patchValue(component['originalFormData']);
      component.patchCompanyForm.markAsPristine();
      
      fixture.detectChanges();
      
      const submitButton = fixture.nativeElement.querySelector('.btn-primary');
      
      // Should be disabled when no changes
      expect(submitButton.disabled).toBeTrue();
      
      // Make a valid change
      component.patchCompanyForm.get('name')?.setValue('Changed Company Name');
      component.patchCompanyForm.get('name')?.markAsDirty();
      fixture.detectChanges();
      
      // Should be enabled when there are valid changes
      expect(submitButton.disabled).toBeFalse();
      
      // Should be disabled when submitting
      component.isSubmitting = true;
      fixture.detectChanges();
      expect(submitButton.disabled).toBeTrue();
      
      // Reset submitting state
      component.isSubmitting = false;
      
      // Make form invalid
      component.patchCompanyForm.get('name')?.setValue('   '); // Invalid whitespace
      component.patchCompanyForm.get('name')?.markAsDirty();
      fixture.detectChanges();
      
      // Should be disabled when form is invalid
      expect(submitButton.disabled).toBeTrue();
    });

    it('should show correct button text when submitting', () => {
      component.isLoading = false;
      fixture.detectChanges();
      
      const submitButton = fixture.nativeElement.querySelector('.btn-primary');
      expect(submitButton.textContent).toContain('Update Company');
      
      component.isSubmitting = true;
      fixture.detectChanges();
      expect(submitButton.textContent).toContain('Updating...');
    });
  });
});