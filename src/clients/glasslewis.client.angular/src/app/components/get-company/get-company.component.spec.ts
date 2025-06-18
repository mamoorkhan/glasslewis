import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap } from '@angular/router';
import { Location } from '@angular/common';
import { of, throwError, Subject } from 'rxjs';
import { GetCompanyComponent } from './get-company.component';
import { GetCompanyService } from '../../services/company/get-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';

describe('GetCompanyComponent', () => {
  let component: GetCompanyComponent;
  let fixture: ComponentFixture<GetCompanyComponent>;
  let mockCompanyService: jasmine.SpyObj<GetCompanyService>;
  let mockLocation: jasmine.SpyObj<Location>;
  let mockActivatedRoute: Partial<ActivatedRoute>;
  let paramsSubject: Subject<Record<string, string>>;

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

  beforeEach(async () => {
    paramsSubject = new Subject();
    
    mockCompanyService = jasmine.createSpyObj('GetCompanyService', ['getCompany']);
    mockLocation = jasmine.createSpyObj('Location', ['back']);
    
    mockActivatedRoute = {
      params: paramsSubject.asObservable(),
      snapshot: {
        params: {},
        title: '',
        url: [],
        queryParams: {},
        fragment: null,
        data: {},
        outlet: 'primary',
        component: null,
        routeConfig: null,
        root: {} as unknown as ActivatedRouteSnapshot,
        parent: null,
        firstChild: null,
        children: [],
        pathFromRoot: [],
        paramMap: {} as ParamMap,
        queryParamMap: {} as ParamMap,
        toString: () => ''
      }
    };

    await TestBed.configureTestingModule({
      imports: [GetCompanyComponent],
      providers: [
        { provide: GetCompanyService, useValue: mockCompanyService },
        { provide: Location, useValue: mockLocation },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GetCompanyComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    paramsSubject.complete();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should load company when valid id is provided', () => {
      mockCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });

      expect(mockCompanyService.getCompany).toHaveBeenCalledWith('123');
      expect(component.company).toEqual(mockCompany);
      expect(component.loading).toBeFalse();
      expect(component.error).toBeNull();
    });

    it('should set error when no id is provided', () => {
      component.ngOnInit();
      paramsSubject.next({});

      expect(mockCompanyService.getCompany).not.toHaveBeenCalled();
      expect(component.error).toBe('Invalid company ID provided');
      expect(component.company).toBeNull();
      expect(component.loading).toBeFalse();
    });

    it('should handle multiple param changes', () => {
      mockCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.ngOnInit();
      
      // First company
      paramsSubject.next({ id: '123' });
      expect(mockCompanyService.getCompany).toHaveBeenCalledWith('123');
      
      // Second company
      paramsSubject.next({ id: '456' });
      expect(mockCompanyService.getCompany).toHaveBeenCalledWith('456');
      expect(mockCompanyService.getCompany).toHaveBeenCalledTimes(2);
    });
  });

  describe('loadCompany', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should successfully load company data', () => {
      mockCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      paramsSubject.next({ id: '123' });

      expect(component.loading).toBeFalse();
      expect(component.error).toBeNull();
      expect(component.company).toEqual(mockCompany);
    });

    it('should set loading state while fetching data', () => {
      const companySubject = new Subject<GetCompanyResponseModel>();
      mockCompanyService.getCompany.and.returnValue(companySubject.asObservable());
      
      paramsSubject.next({ id: '123' });

      expect(component.loading).toBeTrue();
      expect(component.error).toBeNull();
      expect(component.company).toBeNull();
      
      // Complete the request
      companySubject.next(mockCompany);
      companySubject.complete();
      
      expect(component.loading).toBeFalse();
      expect(component.company).toEqual(mockCompany);
    });

    it('should handle 404 error', () => {
      const error = { status: 404, message: 'Not found' };
      mockCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '999' });

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Company not found');
      expect(component.company).toBeNull();
    });

    it('should handle connection error (status 0)', () => {
      const error = { status: 0, message: 'Connection failed' };
      mockCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Unable to connect to the server. Please check your connection.');
      expect(component.company).toBeNull();
    });

    it('should handle generic error with message', () => {
      const error = { status: 500, message: 'Internal server error' };
      mockCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Error loading company details: Internal server error');
      expect(component.company).toBeNull();
    });

    it('should handle generic error without message', () => {
      const error = { status: 500 };
      mockCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(component.loading).toBeFalse();
      expect(component.error).toBe('Error loading company details: Unknown error');
      expect(component.company).toBeNull();
    });

    it('should log error to console', () => {
      spyOn(console, 'error');
      const error = { status: 500, message: 'Server error' };
      mockCompanyService.getCompany.and.returnValue(throwError(() => error));
      
      paramsSubject.next({ id: '123' });

      expect(console.error).toHaveBeenCalledWith('Error loading company:', error);
    });
  });

  describe('goBack', () => {
    it('should call location.back()', () => {
      component.goBack();
      expect(mockLocation.back).toHaveBeenCalled();
    });
  });

  describe('retry', () => {
    it('should reload company with id from route snapshot', () => {
      mockActivatedRoute.snapshot.params = { id: '789' };
      mockCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.retry();

      expect(mockCompanyService.getCompany).toHaveBeenCalledWith('789');
      expect(component.company).toEqual(mockCompany);
    });

    it('should not reload if no id in route snapshot', () => {
      mockActivatedRoute.snapshot.params = {};
      
      component.retry();

      expect(mockCompanyService.getCompany).not.toHaveBeenCalled();
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

    it('should unsubscribe from params observable', () => {
      mockCompanyService.getCompany.and.returnValue(of(mockCompany));
      
      component.ngOnInit();
      paramsSubject.next({ id: '123' });
      
      // Destroy component
      component.ngOnDestroy();
      
      // Reset spy
      mockCompanyService.getCompany.calls.reset();
      
      // Try to emit new params - should not trigger service call
      paramsSubject.next({ id: '456' });
      
      expect(mockCompanyService.getCompany).not.toHaveBeenCalled();
    });
  });

  describe('Template Integration', () => {
    it('should display loading message when loading', () => {
      component.loading = true;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      expect(compiled.querySelector('.loading')).toBeTruthy();
      expect(compiled.textContent).toContain('Loading company details...');
    });

    it('should display error message and retry button when error occurs', () => {
      component.error = 'Test error message';
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      const errorDiv = compiled.querySelector('.error-message');
      const retryButton = compiled.querySelector('.retry-btn');
      
      expect(errorDiv).toBeTruthy();
      expect(errorDiv.textContent).toContain('Test error message');
      expect(retryButton).toBeTruthy();
      expect(retryButton.textContent).toContain('Retry');
    });

    it('should trigger retry when retry button is clicked', () => {
      component.error = 'Test error';
      fixture.detectChanges();
      
      spyOn(component, 'retry');
      const retryButton = fixture.nativeElement.querySelector('.retry-btn');
      retryButton.click();
      
      expect(component.retry).toHaveBeenCalled();
    });

    it('should display company details when company is loaded', () => {
      component.company = mockCompany;
      component.loading = false;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      
      expect(compiled.querySelector('h1').textContent).toContain('Test Company');
      expect(compiled.textContent).toContain('123'); // ID
      expect(compiled.textContent).toContain('TEST'); // Stock ticker
      expect(compiled.textContent).toContain('NYSE'); // Exchange
      expect(compiled.textContent).toContain('US1234567890'); // ISIN
      expect(compiled.querySelector('a[href="https://test.com"]')).toBeTruthy();
    });

    it('should not display website link when website is null', () => {
      component.company = { ...mockCompany, website: null };
      component.loading = false;
      fixture.detectChanges();
      
      const compiled = fixture.nativeElement;
      const websiteLink = compiled.querySelector('a[target="_blank"]');
      
      expect(websiteLink).toBeFalsy();
    });

    it('should trigger goBack when back button is clicked', () => {
      component.company = mockCompany;
      component.loading = false;
      fixture.detectChanges();
      
      spyOn(component, 'goBack');
      const backButton = fixture.nativeElement.querySelector('.btn-primary');
      backButton.click();
      
      expect(component.goBack).toHaveBeenCalled();
    });
  });

  describe('Edge Cases', () => {
    it('should handle rapid parameter changes', () => {
      const companySubject1 = new Subject<GetCompanyResponseModel>();
      const companySubject2 = new Subject<GetCompanyResponseModel>();
      
      mockCompanyService.getCompany
        .withArgs('123').and.returnValue(companySubject1.asObservable())
        .withArgs('456').and.returnValue(companySubject2.asObservable());
      
      component.ngOnInit();
      
      // First request
      paramsSubject.next({ id: '123' });
      expect(component.loading).toBeTrue();
      
      // Second request before first completes
      paramsSubject.next({ id: '456' });
      expect(component.loading).toBeTrue();
      
      // Complete first request (should be ignored due to takeUntil)
      companySubject1.next({ ...mockCompany, id: '123' });
      companySubject1.complete();
      
      // Complete second request
      companySubject2.next({ ...mockCompany, id: '456' });
      companySubject2.complete();
      
      expect(component.company?.id).toBe('456');
    });
  });
});