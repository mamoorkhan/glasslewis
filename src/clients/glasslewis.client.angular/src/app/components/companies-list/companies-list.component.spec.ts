import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CompaniesListComponent } from './companies-list.component';
import { GetCompaniesListService } from '../../services/company/get-companies-list.service';
import { DeleteCompanyService } from '../../services/company/delete-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';

describe('CompaniesListComponent', () => {
  let component: CompaniesListComponent;
  let fixture: ComponentFixture<CompaniesListComponent>;
  let getCompaniesListService: jasmine.SpyObj<GetCompaniesListService>;
  let deleteCompanyService: jasmine.SpyObj<DeleteCompanyService>;
  let router: jasmine.SpyObj<Router>;

  const mockCompanies: GetCompanyResponseModel[] = [
    {
      id: '1',
      name: 'Apple Inc.',
      stockTicker: 'AAPL',
      exchange: 'NASDAQ',
      isin: 'US0378331005',
      website: 'https://www.apple.com',
      createdAt: new Date('2025-01-01T10:00:00Z'),
      updatedAt: new Date('2025-01-01T12:00:00Z')
    },
    {
      id: '2',
      name: 'Microsoft Corporation',
      stockTicker: 'MSFT',
      exchange: 'NASDAQ',
      isin: 'US5949181045',
      website: 'https://www.microsoft.com',
      createdAt: new Date('2025-01-02T10:00:00Z'),
      updatedAt: new Date('2025-01-02T12:00:00Z')
    },
    {
      id: '3',
      name: 'Tesla Inc.',
      stockTicker: 'TSLA',
      exchange: 'NASDAQ',
      isin: 'US88160R1014',
      createdAt: new Date('2025-01-03T10:00:00Z'),
      updatedAt: new Date('2025-01-03T12:00:00Z')
    }
  ];

  beforeEach(async () => {
    const getCompaniesListSpy = jasmine.createSpyObj('GetCompaniesListService', ['getCompaniesList']);
    const deleteCompanySpy = jasmine.createSpyObj('DeleteCompanyService', ['deleteCompany']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, CompaniesListComponent],
      providers: [
        { provide: GetCompaniesListService, useValue: getCompaniesListSpy },
        { provide: DeleteCompanyService, useValue: deleteCompanySpy },
        { provide: Router, useValue: routerSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CompaniesListComponent);
    component = fixture.componentInstance;
    getCompaniesListService = TestBed.inject(GetCompaniesListService) as jasmine.SpyObj<GetCompaniesListService>;
    deleteCompanyService = TestBed.inject(DeleteCompanyService) as jasmine.SpyObj<DeleteCompanyService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  describe('Component Initialization', () => {
    it('should create', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with default values', () => {
      expect(component.companies).toEqual([]);
      expect(component.filteredCompanies).toEqual([]);
      expect(component.isLoading).toBe(false);
      expect(component.errorMessage).toBe('');
      expect(component.searchTerm).toBe('');
      expect(component.sortColumn).toBe('');
      expect(component.sortDirection).toBe('asc');
      expect(component.currentPage).toBe(1);
      expect(component.itemsPerPage).toBe(10);
      expect(component.totalItems).toBe(0);
      expect(component.showDeleteModal).toBe(false);
      expect(component.showViewModal).toBe(false);
      expect(component.showViewByIsinModal).toBe(false);
      expect(component.selectedCompany).toBeNull();
    });

    it('should call loadCompanies on ngOnInit', () => {
      spyOn(component, 'loadCompanies');
      component.ngOnInit();
      expect(component.loadCompanies).toHaveBeenCalled();
    });
  });

  describe('Load Companies', () => {
    it('should load companies successfully', () => {
      getCompaniesListService.getCompaniesList.and.returnValue(of(mockCompanies));

      component.loadCompanies();

      expect(component.isLoading).toBe(false);
      expect(component.companies).toEqual(mockCompanies);
      expect(component.filteredCompanies).toEqual(mockCompanies);
      expect(component.totalItems).toBe(mockCompanies.length);
      expect(component.errorMessage).toBe('');
    });

    it('should handle error when loading companies with error message', () => {
      const errorResponse = { error: { message: 'Service unavailable' } };
      getCompaniesListService.getCompaniesList.and.returnValue(throwError(() => errorResponse));
      spyOn(console, 'error');

      component.loadCompanies();

      expect(component.isLoading).toBe(false);
      expect(component.errorMessage).toBe('Service unavailable');
      expect(console.error).toHaveBeenCalledWith('Error loading companies:', errorResponse);
    });

    it('should handle error when loading companies without specific error message', () => {
      const error = new Error('Network error');
      getCompaniesListService.getCompaniesList.and.returnValue(throwError(() => error));
      spyOn(console, 'error');

      component.loadCompanies();

      expect(component.isLoading).toBe(false);
      expect(component.errorMessage).toBe('Failed to load companies. Please try again.');
      expect(console.error).toHaveBeenCalledWith('Error loading companies:', error);
    });

    it('should set loading state during API call', () => {
      getCompaniesListService.getCompaniesList.and.returnValue(of(mockCompanies));

      expect(component.isLoading).toBe(false);
      component.loadCompanies();
      expect(component.isLoading).toBe(false); // Should be false after completion
    });

    it('should clear error message when loading companies', () => {
      component.errorMessage = 'Previous error';
      getCompaniesListService.getCompaniesList.and.returnValue(of(mockCompanies));

      component.loadCompanies();

      expect(component.errorMessage).toBe('');
    });
  });

  describe('Search Functionality', () => {
    beforeEach(() => {
      component.companies = mockCompanies;
      component.filteredCompanies = [...mockCompanies];
    });

    it('should return all companies when search term is empty', () => {
      component.searchTerm = '';
      component.onSearch();

      expect(component.filteredCompanies).toEqual(mockCompanies);
      expect(component.totalItems).toBe(mockCompanies.length);
      expect(component.currentPage).toBe(1);
    });

    it('should return all companies when search term is only whitespace', () => {
      component.searchTerm = '   ';
      component.onSearch();

      expect(component.filteredCompanies).toEqual(mockCompanies);
      expect(component.totalItems).toBe(mockCompanies.length);
    });

    it('should filter by company name', () => {
      component.searchTerm = 'Apple';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(1);
      expect(component.filteredCompanies[0].name).toBe('Apple Inc.');
      expect(component.currentPage).toBe(1);
    });

    it('should filter by stock ticker', () => {
      component.searchTerm = 'MSFT';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(1);
      expect(component.filteredCompanies[0].stockTicker).toBe('MSFT');
    });

    it('should filter by exchange', () => {
      component.searchTerm = 'NASDAQ';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(3);
    });

    it('should filter by ISIN', () => {
      component.searchTerm = 'US0378331005';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(1);
      expect(component.filteredCompanies[0].isin).toBe('US0378331005');
    });

    it('should perform case-insensitive search', () => {
      component.searchTerm = 'apple';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(1);
      expect(component.filteredCompanies[0].name).toBe('Apple Inc.');
    });

    it('should return empty array when no matches found', () => {
      component.searchTerm = 'nonexistent';
      component.onSearch();

      expect(component.filteredCompanies.length).toBe(0);
      expect(component.totalItems).toBe(0);
    });
  });

  describe('Sorting Functionality', () => {
    beforeEach(() => {
      component.filteredCompanies = [...mockCompanies];
    });

    it('should sort by string column (name) in ascending order', () => {
      component.sort('name');

      expect(component.sortColumn).toBe('name');
      expect(component.sortDirection).toBe('asc');
      expect(component.filteredCompanies[0].name).toBe('Apple Inc.');
      expect(component.filteredCompanies[1].name).toBe('Microsoft Corporation');
      expect(component.filteredCompanies[2].name).toBe('Tesla Inc.');
    });

    it('should toggle sort direction when clicking same column', () => {
      component.sortColumn = 'name';
      component.sortDirection = 'asc';

      component.sort('name');

      expect(component.sortDirection).toBe('desc');
      expect(component.filteredCompanies[0].name).toBe('Tesla Inc.');
      expect(component.filteredCompanies[1].name).toBe('Microsoft Corporation');
      expect(component.filteredCompanies[2].name).toBe('Apple Inc.');
    });

    it('should sort by date column (createdAt) in ascending order', () => {
      component.sort('createdAt');

      expect(component.sortColumn).toBe('createdAt');
      expect(component.sortDirection).toBe('asc');
      expect(component.filteredCompanies[0].id).toBe('1'); // Earliest date
      expect(component.filteredCompanies[2].id).toBe('3'); // Latest date
    });

    it('should sort by date column (updatedAt) in descending order', () => {
      component.sort('updatedAt');
      component.sort('updatedAt'); // Toggle to desc

      expect(component.sortDirection).toBe('desc');
      expect(component.filteredCompanies[0].id).toBe('3'); // Latest updated
      expect(component.filteredCompanies[2].id).toBe('1'); // Earliest updated
    });

    it('should handle string comparison correctly', () => {
      component.sort('stockTicker');

      expect(component.filteredCompanies[0].stockTicker).toBe('AAPL');
      expect(component.filteredCompanies[1].stockTicker).toBe('MSFT');
      expect(component.filteredCompanies[2].stockTicker).toBe('TSLA');
    });

    it('should reset sort direction when sorting different column', () => {
      component.sortColumn = 'name';
      component.sortDirection = 'desc';

      component.sort('stockTicker');

      expect(component.sortColumn).toBe('stockTicker');
      expect(component.sortDirection).toBe('asc');
    });

    it('should handle equal values in sorting', () => {
      const equalCompanies = [
        { ...mockCompanies[0], name: 'Same Name' },
        { ...mockCompanies[1], name: 'Same Name' },
        { ...mockCompanies[2], name: 'Different Name' }
      ];

      component.filteredCompanies = equalCompanies;
      component.sort('name');

      // Should handle equal values gracefully
      expect(component.filteredCompanies).toHaveSize(3);
    });
  });

  describe('Pagination', () => {
    beforeEach(() => {
      component.filteredCompanies = mockCompanies;
      component.totalItems = mockCompanies.length;
      component.itemsPerPage = 2;
    });

    it('should calculate total pages correctly', () => {
      expect(component.totalPages).toBe(2); // 3 items / 2 per page = 2 pages
    });

    it('should return correct paginated companies for first page', () => {
      component.currentPage = 1;
      const paginatedCompanies = component.paginatedCompanies;

      expect(paginatedCompanies.length).toBe(2);
      expect(paginatedCompanies[0].id).toBe('1');
      expect(paginatedCompanies[1].id).toBe('2');
    });

    it('should return correct paginated companies for second page', () => {
      component.currentPage = 2;
      const paginatedCompanies = component.paginatedCompanies;

      expect(paginatedCompanies.length).toBe(1);
      expect(paginatedCompanies[0].id).toBe('3');
    });

    it('should go to valid page', () => {
      component.goToPage(2);
      expect(component.currentPage).toBe(2);
    });

    it('should not go to page less than 1', () => {
      component.currentPage = 1;
      component.goToPage(0);
      expect(component.currentPage).toBe(1);
    });

    it('should not go to page greater than total pages', () => {
      component.currentPage = 1;
      component.goToPage(10);
      expect(component.currentPage).toBe(1);
    });

    it('should handle edge case with itemsPerPage larger than total items', () => {
      component.itemsPerPage = 10;
      component.currentPage = 1;
      const paginatedCompanies = component.paginatedCompanies;

      expect(paginatedCompanies.length).toBe(3);
      expect(component.totalPages).toBe(1);
    });
  });

  describe('CRUD Operations', () => {
    const mockCompany = mockCompanies[0];

    it('should handle onView', () => {
      spyOn(console, 'log');
      component.onView(mockCompany);

      expect(component.selectedCompany).toBe(mockCompany);
      expect(component.showViewModal).toBe(true);
      expect(console.log).toHaveBeenCalledWith('Viewing company:', mockCompany);
    });

    it('should handle onViewByIsin', () => {
      spyOn(console, 'log');
      component.onViewByIsin(mockCompany);

      expect(component.selectedCompany).toBe(mockCompany);
      expect(component.showViewByIsinModal).toBe(true);
      expect(console.log).toHaveBeenCalledWith('Viewing company by ISIN:', mockCompany.isin);
    });

    it('should handle onUpdate', () => {
      spyOn(console, 'log');
      component.onUpdate(mockCompany);

      expect(console.log).toHaveBeenCalledWith('Updating company:', mockCompany);
      expect(router.navigate).toHaveBeenCalledWith(['/company/update', mockCompany.id]);
    });

    it('should handle onPatch', () => {
      spyOn(console, 'log');
      component.onPatch(mockCompany);

      expect(console.log).toHaveBeenCalledWith('Patching company:', mockCompany);
      expect(router.navigate).toHaveBeenCalledWith(['/company/patch', mockCompany.id]);
    });

    it('should handle onDelete', () => {
      component.onDelete(mockCompany);

      expect(component.selectedCompany).toBe(mockCompany);
      expect(component.showDeleteModal).toBe(true);
    });

    it('should handle onCreate', () => {
      spyOn(console, 'log');
      component.onCreate();

      expect(console.log).toHaveBeenCalledWith('Creating new company');
      expect(router.navigate).toHaveBeenCalledWith(['/company/create']);
      expect(component.showViewModal).toBe(false);
    });
  });

  describe('Delete Functionality', () => {
    const mockCompany = mockCompanies[0];

    it('should confirm delete successfully', () => {
      component.selectedCompany = mockCompany;
      deleteCompanyService.deleteCompany.and.returnValue(of(null));
      spyOn(component, 'loadCompanies');
      spyOn(component, 'closeDeleteModal');
      spyOn(console, 'log');

      component.confirmDelete();

      expect(console.log).toHaveBeenCalledWith('Deleting company:', mockCompany);
      expect(deleteCompanyService.deleteCompany).toHaveBeenCalledWith(mockCompany.id);
      expect(component.loadCompanies).toHaveBeenCalled();
      expect(component.closeDeleteModal).toHaveBeenCalled();
    });

    it('should handle delete error with specific message', () => {
      component.selectedCompany = mockCompany;
      const errorResponse = { error: { message: 'Cannot delete company' } };
      deleteCompanyService.deleteCompany.and.returnValue(throwError(() => errorResponse));
      spyOn(component, 'closeDeleteModal');
      spyOn(console, 'error');

      component.confirmDelete();

      expect(component.errorMessage).toBe('Cannot delete company');
      expect(component.isLoading).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error loading companies:', errorResponse);
      expect(component.closeDeleteModal).toHaveBeenCalled();
    });

    it('should handle delete error without specific message', () => {
      component.selectedCompany = mockCompany;
      const error = new Error('Network error');
      deleteCompanyService.deleteCompany.and.returnValue(throwError(() => error));
      spyOn(component, 'closeDeleteModal');
      spyOn(console, 'error');

      component.confirmDelete();

      expect(component.errorMessage).toBe('Failed to delete company. Please try again.');
      expect(component.isLoading).toBe(false);
      expect(console.error).toHaveBeenCalledWith('Error loading companies:', error);
      expect(component.closeDeleteModal).toHaveBeenCalled();
    });

    it('should handle confirmDelete when selectedCompany is null', () => {
      component.selectedCompany = null;
      spyOn(component, 'closeDeleteModal');

      component.confirmDelete();

      expect(deleteCompanyService.deleteCompany).not.toHaveBeenCalled();
      expect(component.closeDeleteModal).toHaveBeenCalled();
    });
  });

  describe('Navigation Methods', () => {
    const mockCompany = mockCompanies[0];

    it('should navigate to company detail', () => {
      component.selectedCompany = mockCompany;
      spyOn(component, 'closeViewModal');

      component.navigateToCompanyDetail();

      expect(router.navigate).toHaveBeenCalledWith(['/company', mockCompany.id]);
      expect(component.closeViewModal).toHaveBeenCalled();
    });

    it('should not navigate when selectedCompany is null', () => {
      component.selectedCompany = null;
      spyOn(component, 'closeViewModal');

      component.navigateToCompanyDetail();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(component.closeViewModal).not.toHaveBeenCalled();
    });

    it('should navigate to company detail by ISIN', () => {
      component.selectedCompany = mockCompany;
      spyOn(component, 'closeViewModal');

      component.navigateToCompanyDetailByIsin();

      expect(router.navigate).toHaveBeenCalledWith(['/company/isin', mockCompany.isin]);
      expect(component.closeViewModal).toHaveBeenCalled();
    });

    it('should not navigate by ISIN when selectedCompany is null', () => {
      component.selectedCompany = null;
      spyOn(component, 'closeViewModal');

      component.navigateToCompanyDetailByIsin();

      expect(router.navigate).not.toHaveBeenCalled();
      expect(component.closeViewModal).not.toHaveBeenCalled();
    });
  });

  describe('Modal Controls', () => {
    it('should close view modal', () => {
      component.showViewModal = true;
      component.selectedCompany = mockCompanies[0];

      component.closeViewModal();

      expect(component.showViewModal).toBe(false);
      expect(component.selectedCompany).toBeNull();
    });

    it('should close view by ISIN modal', () => {
      component.showViewByIsinModal = true;
      component.selectedCompany = mockCompanies[0];

      component.closeViewByIsinModal();

      expect(component.showViewByIsinModal).toBe(false);
      expect(component.selectedCompany).toBeNull();
    });

    it('should close delete modal', () => {
      component.showDeleteModal = true;
      component.selectedCompany = mockCompanies[0];

      component.closeDeleteModal();

      expect(component.showDeleteModal).toBe(false);
      expect(component.selectedCompany).toBeNull();
    });
  });

  describe('ISIN Utility Methods', () => {
    it('should extract country from ISIN', () => {
      expect(component.getCountryFromIsin('US0378331005')).toBe('US');
      expect(component.getCountryFromIsin('GB00B4LB8F41')).toBe('GB');
    });

    it('should return Unknown for invalid ISIN country', () => {
      expect(component.getCountryFromIsin('')).toBe('Unknown');
      expect(component.getCountryFromIsin('A')).toBe('Unknown');
      expect(component.getCountryFromIsin(null as unknown as string)).toBe('Unknown');
    });

    it('should extract security ID from ISIN', () => {
      expect(component.getSecurityIdFromIsin('US0378331005')).toBe('037833100');
      expect(component.getSecurityIdFromIsin('GB00B4LB8F41')).toBe('00B4LB8F4');
    });

    it('should return Unknown for invalid ISIN security ID', () => {
      expect(component.getSecurityIdFromIsin('')).toBe('Unknown');
      expect(component.getSecurityIdFromIsin('US037833')).toBe('Unknown');
      expect(component.getSecurityIdFromIsin(null as unknown as string)).toBe('Unknown');
    });

    it('should extract check digit from ISIN', () => {
      expect(component.getCheckDigitFromIsin('US0378331005')).toBe('5');
      expect(component.getCheckDigitFromIsin('GB00B4LB8F41')).toBe('1');
    });

    it('should return Unknown for invalid ISIN check digit', () => {
      expect(component.getCheckDigitFromIsin('')).toBe('Unknown');
      expect(component.getCheckDigitFromIsin('US037833100')).toBe('Unknown');
      expect(component.getCheckDigitFromIsin(null as unknown as string)).toBe('Unknown');
    });
  });

  describe('Utility Methods', () => {
    it('should format date correctly', () => {
      const date = new Date('2025-06-10T14:30:00Z');
      const formatted = component.formatDate(date);
      
      expect(formatted).toContain('Jun');
      expect(formatted).toContain('10');
      expect(formatted).toContain('2025');
    });

    it('should format date string correctly', () => {
      const dateString = '2025-06-10T14:30:00Z';
      const formatted = component.formatDate(dateString);
      
      expect(formatted).toContain('Jun');
      expect(formatted).toContain('10');
      expect(formatted).toContain('2025');
    });

    it('should return correct sort icon for ascending', () => {
      component.sortColumn = 'name';
      component.sortDirection = 'asc';

      expect(component.getSortIcon('name')).toBe('[↑]');
      expect(component.getSortIcon('stockTicker')).toBe('');
    });

    it('should return correct sort icon for descending', () => {
      component.sortColumn = 'name';
      component.sortDirection = 'desc';

      expect(component.getSortIcon('name')).toBe('[↓]');
      expect(component.getSortIcon('stockTicker')).toBe('');
    });

    it('should return empty string for non-sorted column', () => {
      component.sortColumn = 'name';

      expect(component.getSortIcon('stockTicker')).toBe('');
    });

    it('should refresh data', () => {
      spyOn(component, 'loadCompanies');
      component.refresh();
      expect(component.loadCompanies).toHaveBeenCalled();
    });
  });

  describe('Edge Cases and Error Handling', () => {
    it('should handle empty companies array', () => {
      component.companies = [];
      component.onSearch();

      expect(component.filteredCompanies).toEqual([]);
      expect(component.totalItems).toBe(0);
    });

    it('should handle pagination with zero items', () => {
      component.filteredCompanies = [];
      component.totalItems = 0;

      expect(component.totalPages).toBe(0);
      expect(component.paginatedCompanies).toEqual([]);
    });

    it('should handle malformed date in sorting', () => {
      const companiesWithBadDate = [
        { ...mockCompanies[0], createdAt: 'invalid-date' as unknown as Date },
        { ...mockCompanies[1] }
      ];

      component.filteredCompanies = companiesWithBadDate;
      
      // Should not throw error
      expect(() => component.sort('createdAt')).not.toThrow();
    });

    it('should handle undefined/null values in sorting', () => {
      const companiesWithNulls = [
        { ...mockCompanies[0], name: null as unknown as string },
        { ...mockCompanies[1], name: undefined as unknown as string },
        { ...mockCompanies[2] }
      ];

      component.filteredCompanies = companiesWithNulls;
      
      // Should not throw error
      expect(() => component.sort('name')).not.toThrow();
    });

    it('should handle very large pagination values', () => {
      component.itemsPerPage = 1000000;
      component.filteredCompanies = mockCompanies;
      component.totalItems = mockCompanies.length;

      expect(component.totalPages).toBe(1);
      expect(component.paginatedCompanies.length).toBe(3);
    });
  });
});