import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class DeleteCompanyService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    deleteCompany(id: string) {
        return this.http.delete(`${this.url}/${id}`);
    }
}