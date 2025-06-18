import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';

import { GetCompaniesListService } from '../../services/company/get-companies-list.service';
import { DeleteCompanyService } from '../../services/company/delete-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';

@Component({
  selector: 'app-companies-list',
  imports: [CommonModule, FormsModule],
  templateUrl: './companies-list.component.html',
  styleUrls: ['./companies-list.component.css']
})
export class CompaniesListComponent implements OnInit {
  companies: GetCompanyResponseModel[] = [];
  filteredCompanies: GetCompanyResponseModel[] = [];
  isLoading = false;
  errorMessage = '';
  searchTerm = '';
  sortColumn = '';
  sortDirection: 'asc' | 'desc' = 'asc';
  
  // Pagination
  currentPage = 1;
  itemsPerPage = 10;
  totalItems = 0;

  // Modal states
  showDeleteModal = false;
  showViewModal = false;
  showViewByIsinModal = false;
  selectedCompany: GetCompanyResponseModel | null = null;

  getCompaniesListService = inject(GetCompaniesListService);
  deleteCompanyService = inject(DeleteCompanyService);
  router = inject(Router);

  ngOnInit(): void {
    this.loadCompanies();
  }

  loadCompanies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.getCompaniesListService.getCompaniesList().subscribe({
      next: (companies: GetCompanyResponseModel[]) => {
        this.companies = companies;
        this.filteredCompanies = [...companies];
        this.totalItems = companies.length;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Failed to load companies. Please try again.';
        this.isLoading = false;
        console.error('Error loading companies:', error);
      }
    });
  }

  // Search functionality
  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredCompanies = [...this.companies];
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredCompanies = this.companies.filter(company =>
        company.name.toLowerCase().includes(term) ||
        company.stockTicker.toLowerCase().includes(term) ||
        company.exchange.toLowerCase().includes(term) ||
        company.isin.toLowerCase().includes(term)
      );
    }
    this.totalItems = this.filteredCompanies.length;
    this.currentPage = 1;
  }

// Sorting functionality
sort(column: keyof GetCompanyResponseModel): void {
  if (this.sortColumn === column) {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
  } else {
    this.sortColumn = column;
    this.sortDirection = 'asc';
  }

  this.filteredCompanies.sort((a, b) => {
    const aValue = a[column];
    const bValue = b[column];

    // Handle date sorting specifically for date columns
    if (column === 'createdAt' || column === 'updatedAt') {
      const aTime = new Date(aValue as Date | string).getTime();
      const bTime = new Date(bValue as Date | string).getTime();
      
      if (aTime < bTime) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (aTime > bTime) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    }

    // Handle string sorting for non-date columns
    if (typeof aValue === 'string' && typeof bValue === 'string') {
      const aStr = aValue.toLowerCase();
      const bStr = bValue.toLowerCase();
      
      if (aStr < bStr) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (aStr > bStr) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    }

    // Fallback comparison
    if (aValue < bValue) {
      return this.sortDirection === 'asc' ? -1 : 1;
    }
    if (aValue > bValue) {
      return this.sortDirection === 'asc' ? 1 : -1;
    }
    return 0;
  });
}

  // Pagination
  get paginatedCompanies(): GetCompanyResponseModel[] {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredCompanies.slice(startIndex, endIndex);
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  // CRUD Operations
  onView(company: GetCompanyResponseModel): void {
    this.selectedCompany = company;
    this.showViewModal = true;
    console.log('Viewing company:', company);
  }

  onViewByIsin(company: GetCompanyResponseModel): void {
    this.selectedCompany = company;
    this.showViewByIsinModal = true;
    console.log('Viewing company by ISIN:', company.isin);
  }

  onUpdate(company: GetCompanyResponseModel): void {
    // Navigate to update component or open update modal
    console.log('Updating company:', company);
    this.router.navigate(['/company/update', company.id]);
  }

  onPatch(company: GetCompanyResponseModel): void {
    // Implement partial update functionality
    console.log('Patching company:', company);
    this.router.navigate(['/company/patch', company.id]);
  }

  onDelete(company: GetCompanyResponseModel): void {
    this.selectedCompany = company;
    this.showDeleteModal = true;
  }

  confirmDelete(): void {
    if (this.selectedCompany) {
      // Implement delete functionality
      console.log('Deleting company:', this.selectedCompany);
      this.deleteCompanyService.deleteCompany(this.selectedCompany.id).subscribe({
        next: () => {
          this.loadCompanies();
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Failed to delete company. Please try again.';
          this.isLoading = false;
          console.error('Error loading companies:', error);
        }
      });      
    }
    this.closeDeleteModal();
  }

  onCreate(): void {
    // Navigate to create component
    console.log('Creating new company');
    this.router.navigate(['/company/create']);
    this.closeViewModal();
  }

  // Navigation method for company details
  navigateToCompanyDetail(): void {
    if (this.selectedCompany) {
      this.router.navigate(['/company', this.selectedCompany.id]);
      this.closeViewModal();
    }
  }

    // Navigation method for company details by ISIN
  navigateToCompanyDetailByIsin(): void {
    if (this.selectedCompany) {
      this.router.navigate(['/company/isin', this.selectedCompany.isin]);
      this.closeViewModal();
    }
  }

  // Modal controls
  closeViewModal(): void {
    this.showViewModal = false;
    this.selectedCompany = null;
  }

  closeViewByIsinModal(): void {
    this.showViewByIsinModal = false;
    this.selectedCompany = null;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.selectedCompany = null;
  }

  // ISIN utility methods
  getCountryFromIsin(isin: string): string {
    if (!isin || isin.length < 2) return 'Unknown';
    return isin.substring(0, 2);
  }

  getSecurityIdFromIsin(isin: string): string {
    if (!isin || isin.length < 11) return 'Unknown';
    return isin.substring(2, 11);
  }

  getCheckDigitFromIsin(isin: string): string {
    if (!isin || isin.length < 12) return 'Unknown';
    return isin.substring(11, 12);
  }

  // Utility methods
  formatDate(date: Date | string): string {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getSortIcon(column: string): string {
    if (this.sortColumn !== column) return '';
    return this.sortDirection === 'asc' ? '[↑]' : '[↓]';
  }

  refresh(): void {
    this.loadCompanies();
  }
}