enum EOrderRecordStatus {
  Created = 0,
  Packaged = 1,
  Assigned = 2,
  Done = 3,
  Canceled = 4,
}

export const orderRecordStatusMap: Record<EOrderRecordStatus, string> = {
  [EOrderRecordStatus.Created]: "Создан",
  [EOrderRecordStatus.Packaged]: "Собран",
  [EOrderRecordStatus.Assigned]: "Назначен",
  [EOrderRecordStatus.Done]: "Выполнен",
  [EOrderRecordStatus.Canceled]: "Отменен",
}

export default EOrderRecordStatus;