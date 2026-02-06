import { setLocale } from "yup";

const tr_messages = {
  mixed: {
    default: "Geçersiz alan",
    required: "${path} zorunlu bir alandır",
    oneOf: "${path} şunlardan biri olmalıdır: ${values}",
    notOneOf: "${path} şunlardan biri olmamalıdır: ${values}",
  },
  string: {
    length: "${path} ${length} karakter olmalıdır",
    min: "${path} en az ${min} karakter olmalıdır",
    max: "${path} en fazla ${max} karakter olmalıdır",
    email: "${path} geçerli bir e-posta olmalıdır",
    url: "${path} geçerli bir URL olmalıdır",
    trim: "${path} kırpılmış olmalıdır",
    lowercase: "${path} küçük harf olmalıdır",
    uppercase: "${path} büyük harf olmalıdır",
  },
  number: {
    min: "${path} en az ${min} olmalıdır",
    max: "${path} en fazla ${max} olmalıdır",
    lessThan: "${path} ${less} değerinden küçük olmalıdır",
    moreThan: "${path} ${more} değerinden büyük olmalıdır",
    positive: "${path} pozitif bir sayı olmalıdır",
    negative: "${path} negatif bir sayı olmalıdır",
    integer: "${path} bir tam sayı olmalıdır",
  },
  date: {
    min: "${path} ${min} tarihinden sonra olmalıdır",
    max: "${path} ${max} tarihinden önce olmalıdır",
  },
  boolean: {},
  object: {
    noUnknown: "${path} bilinmeyen anahtarlar içeremez",
  },
  array: {
    min: "${path} en az ${min} öğe içermelidir",
    max: "${path} en fazla ${max} öğe içermelidir",
  },
};

export const setYupLocale = (locale: string) => {
  if (locale === "tr") {
    setLocale(tr_messages);
  }
  if (locale === "en") {
    setLocale({});
  }
};