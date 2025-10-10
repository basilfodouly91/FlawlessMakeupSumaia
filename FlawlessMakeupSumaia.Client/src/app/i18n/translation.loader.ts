import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

export function HttpLoaderFactory() {
    return {
        getTranslation: (lang: string) => {
            return new Promise<any>((resolve) => {
                import(`../../assets/i18n/${lang}.json`).then((module) => {
                    resolve(module.default);
                });
            });
        }
    };
}
