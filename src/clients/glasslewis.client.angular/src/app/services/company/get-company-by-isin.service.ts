import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GetCompanyResponseModel } from '../../models/response/get-company-response.model';

@Injectable({
    providedIn: 'root'
})
export class GetCompanyByIsinService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    getCompanyByIsin(isin: string) {
        return this.http.get<GetCompanyResponseModel>(`${this.url}/isin/${isin}`);
    }
}