import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';

@Injectable({
    providedIn: 'root'
})
export class GetCompanyService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    getCompany(id: string) {
        return this.http.get<GetCompanyResponseModel>(`${this.url}/${id}`);
    }
}