import { protectedResources } from '../../auth-config';
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UpdateCompanyRequestModel } from '../../models/request/update-company-request.model';
import { UpdateCompanyResponseModel } from '../../models/response/update-company-response.model';

@Injectable({
    providedIn: 'root'
})
export class UpdateCompanyService {
    url = protectedResources.companyAPI.endpoint;

    http = inject(HttpClient);

    editCompany(id: string, updateCompanyRequestModel: UpdateCompanyRequestModel) {
        return this.http.put<UpdateCompanyResponseModel>(`${this.url}/${id}`, updateCompanyRequestModel);
    }
}