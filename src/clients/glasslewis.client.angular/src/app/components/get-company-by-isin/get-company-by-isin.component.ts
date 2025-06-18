import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { GetCompanyByIsinService } from '../../services/company/get-company-by-isin.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { Location } from '@angular/common';

@Component({
  imports: [CommonModule],
  selector: 'app-get-company-by-isin',
  templateUrl: './get-company-by-isin.component.html',
  styleUrls: ['./get-company-by-isin.component.css']
})
export class GetCompanyByIsinComponent implements OnInit, OnDestroy {
  company: GetCompanyResponseModel | null = null;
  loading = false;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  route = inject(ActivatedRoute);
  getCompanyByIsinService = inject(GetCompanyByIsinService);
  location = inject(Location);

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const isin = params['isin'];
        if (isin) {
          this.loadCompany(isin);
        } else {
          this.error = 'Invalid company ISIN provided';
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCompany(isin: string): void {
    this.loading = true;
    this.error = null;
    this.company = null;

    this.getCompanyByIsinService.getCompanyByIsin(isin)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.company = response;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading company:', error);
          this.loading = false;
          
          if (error.status === 404) {
            this.error = 'Company not found';
          } else if (error.status === 0) {
            this.error = 'Unable to connect to the server. Please check your connection.';
          } else {
            this.error = `Error loading company details: ${error.message || 'Unknown error'}`;
          }
        }
      });
  }

  goBack(): void {
    this.location.back();
  }

  retry(): void {
    const isin = this.route.snapshot.params['isin'];
    if (isin) {
      this.loadCompany(isin);
    }
  }
}