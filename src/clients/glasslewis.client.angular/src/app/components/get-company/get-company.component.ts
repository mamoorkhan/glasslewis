import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { GetCompanyService } from '../../services/company/get-company.service';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';
import { Location } from '@angular/common';

@Component({
  imports: [CommonModule],
  selector: 'app-get-company',
  templateUrl: './get-company.component.html',
  styleUrls: ['./get-company.component.css']
})
export class GetCompanyComponent implements OnInit, OnDestroy {
  company: GetCompanyResponseModel | null = null;
  loading = false;
  error: string | null = null;
  private destroy$ = new Subject<void>();

  private route = inject(ActivatedRoute);
  private companyService = inject(GetCompanyService);
  private location = inject(Location);

  ngOnInit(): void {
    this.route.params
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = params['id'];
        if (id) {
          this.loadCompany(id);
        } else {
          this.error = 'Invalid company ID provided';
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCompany(id: string): void {
    this.loading = true;
    this.error = null;
    this.company = null;

    this.companyService.getCompany(id)
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
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.loadCompany(id);
    }
  }
}