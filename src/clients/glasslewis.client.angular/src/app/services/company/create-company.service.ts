import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateCompanyRequestModel } from '../../models/request/create-company-request.model';
import { CreateCompanyResponseModel } from '../../models/response/create-company-response.model';

@Injectable({
    providedIn: 'root'
})
export class CreateCompanyService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    createCompany(createCompanyRequestModel: CreateCompanyRequestModel) {
        return this.http.post<CreateCompanyResponseModel>(this.url, createCompanyRequestModel);
    }
}